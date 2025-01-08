import org.apache.tools.ant.taskdefs.condition.Os
import org.jetbrains.changelog.Changelog
import org.jetbrains.changelog.ChangelogSectionUrlBuilder
import org.jetbrains.changelog.date
import org.jetbrains.changelog.markdownToHTML

plugins {
    id("java")
    id("org.jetbrains.changelog") version "2.2.1"
    id("org.jetbrains.intellij.platform")
    kotlin("jvm")
}

repositories {
    maven("https://cache-redirector.jetbrains.com/intellij-dependencies")
    maven("https://cache-redirector.jetbrains.com/intellij-repository/releases")
    maven("https://cache-redirector.jetbrains.com/intellij-repository/snapshots")
    maven("https://cache-redirector.jetbrains.com/maven-central")
    intellijPlatform {
        defaultRepositories()
        jetbrainsRuntime()
    }
}

apply {
    plugin("kotlin")
}

val productVersion = extra["productVersion"].toString()
val buildConfiguration = extra["buildConfiguration"].toString()

val repoRoot = projectDir.parentFile!!
val backendRoot = File(repoRoot, "resharper")


java {
    sourceCompatibility = JavaVersion.VERSION_17
    targetCompatibility = JavaVersion.VERSION_17
}


sourceSets {
    main {
        java {
            srcDir("src/main/gen")
            srcDir("src/main/rdgen/kotlin")
        }
        resources {
            srcDir("src/main/rdgen/resources")
        }
    }
}

idea {
    module {
        generatedSourceDirs.add(file("src/main/rdgen/kotlin"))
        resourceDirs.add(file("src/main/rdgen/resources"))
    }
}

dependencies {
    intellijPlatform {
        rider(productVersion)
        jetbrainsRuntime()
        instrumentationTools()
        pluginVerifier()
        zipSigner()
    }
}

tasks {
    changelog {
        version.set(project.version.toString())
        path.set("${project.projectDir}/../CHANGELOG.md")
        header.set(provider { "${version.get()} (${date()})" })
        unreleasedTerm.set("[Unreleased]")
        groups.set(listOf("Added", "Fixed", "Changed", "Removed"))
        sectionUrlBuilder.set(ChangelogSectionUrlBuilder { repositoryUrl, currentVersion, previousVersion, isUnreleased -> "unreleased" })
    }
}


intellijPlatform {
    pluginConfiguration {
        // Extract the <!-- Plugin description --> section from README.md and provide for the plugin's manifest
        description = providers.fileContents(layout.projectDirectory.file("../README.md")).asText.map {
            val startMark = "<!-- plugin description start -->"
            val endMark = "<!-- plugin description end -->"


            var catch = false
            var lines = mutableListOf<String>()
            for (line in it.lines()) {
                if (line.startsWith(endMark)) catch = false
                if (catch) lines.add(line)
                if (line.startsWith(startMark)) catch = true

            }

            lines.joinToString("\n").let(::markdownToHTML)

        }

        val changelog = project.changelog // local variable for configuration cache compatibility
        // Get the latest available change notes from the changelog file
        changeNotes = with(changelog) {
            renderItem(
                (getOrNull(project.version.toString()) ?: getUnreleased()).withHeader(false).withEmptySections(false),
                Changelog.OutputType.HTML,
            )

        }

        ideaVersion {
            sinceBuild = "243.3.3"
            untilBuild = "243.*"
        }


    }

    publishing {
        token = providers.environmentVariable("JB_MARKETPLACE_TOKEN")
        channels = listOf("Stable")
    }

    pluginVerification {
        ides {
            recommended()
        }
    }

}




fun getDotnetCli(): String {
    val pathComponents = System.getenv("PATH").split(File.pathSeparator)
    var dotnetCli: String? = null
    for (dir in pathComponents) {
        val dotNetCliFile = File(
            dir, if (Os.isFamily(Os.FAMILY_WINDOWS)) {
                "dotnet.exe"
            } else {
                "dotnet"
            }
        )
        if (dotNetCliFile.exists()) {
            logger.info("dotNetCliPath: ${dotNetCliFile.canonicalPath}")
            project.extra["dotNetCliPath"] = dotNetCliFile.canonicalPath
            dotnetCli = dotNetCliFile.canonicalPath
            break
        }
    }

    if (dotnetCli == null) {
        error(".NET Core CLI not found. Please add 'dotnet' to PATH")
    }
    return dotnetCli
}

tasks {
    processResources {

    }

    compileKotlin {
        kotlinOptions {
            jvmTarget = "17"
        }
    }

    runIde {
        maxHeapSize = "1500m"
    }


    patchPluginXml {
        dependsOn(patchChangelog)
    }


    val buildBackend by registering {
        doLast {

            val dotnetCli = getDotnetCli()


            val file = File(backendRoot, "AbpInsight/AbpInsight.Rider.csproj")

            val buildArguments = listOf(
                "build", file.canonicalPath, "/p:Configuration=$buildConfiguration", "/p:Version=$version", "/nologo"
            )

            logger.info("dotnet call: '$dotnetCli' '$buildArguments' in '${file.parent}'")

            exec {
                executable = dotnetCli
                args = buildArguments
                workingDir = file.parentFile
            }
        }
    }


    val packBackend by registering {

        doLast {

            val file = File(backendRoot, "AbpInsight")

            val packArguments = listOf(
                "pack",
                file.canonicalPath,
                "/p:Configuration=$buildConfiguration",
                "/p:PackageOutputPath=${buildDir}/distributions/${buildConfiguration}",
                "/p:NuspecFile=${rootDir}/src/dotnet/AbpInsight/AbpInsight.nuspec",
                "/p:PackageVersion=$version",
            )

            val dotnetCli = getDotnetCli()

            logger.info("dotnet pack: '$dotnetCli' '$packArguments' in '${file.parent}'")

            exec {
                executable = dotnetCli
                args = packArguments
                workingDir = file.parentFile
            }
        }
    }

    prepareSandbox {
        dependsOn(buildBackend)
        val outputFolder = "${backendRoot}/AbpInsight/bin/AbpInsight.Rider/${buildConfiguration}"
        val dllFiles = listOf(
            "${outputFolder}/AbpInsight.Rider.dll",
            "${outputFolder}/AbpInsight.Rider.pdb",
        )
        dllFiles.forEach {
            from(it) { into("AbpInsight/dotnet") }
        }

        from("${backendRoot}/AbpInsight/annotations") {
            into("AbpInsight/dotnet/Extensions/Carbon.AbpInsight/annotations")
        }

        doLast {
            dllFiles.forEach {
                val file = file(it)
                if (file.exists().not()) {
                    error("File $file does not exist")
                }
            }
        }
    }

    register("release") {
        dependsOn(patchChangelog)

        description = "create the release commit and tag"

        doLast {
            exec {
                commandLine("git", "commit", "-m", "release $version", "-a")
            }
            exec {
                commandLine("git", "tag", version.toString())
            }
        }

    }

}



