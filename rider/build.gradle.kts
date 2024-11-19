import org.apache.tools.ant.taskdefs.condition.Os
import org.jetbrains.intellij.IntelliJPluginConstants
import org.jetbrains.intellij.tasks.SetupDependenciesTask

plugins {
    id("java")
    id("com.jetbrains.rdgen")
    id("org.jetbrains.intellij") version "1.15.0"
    kotlin("jvm") version "1.9.0"
}

repositories {
    maven { setUrl("https://cache-redirector.jetbrains.com/intellij-repository/snapshots") }
    maven { setUrl("https://cache-redirector.jetbrains.com/maven-central") }
    mavenCentral()
}

apply {
    plugin("kotlin")
}

val productVersion = extra["productVersion"].toString()
val buildConfiguration = extra["buildConfiguration"].toString()


val rdLibDirectory: () -> File = { file("${tasks.setupDependencies.get().idea.get().classes}/lib/rd") }
extra["rdLibDirectory"] = rdLibDirectory

val repoRoot = projectDir.parentFile!!
val backendRoot = File(repoRoot, "resharper")

version = "0.0.1"


sourceSets {
    main {
        java {
            srcDir("src/rider/main/java")
        }
        kotlin {
            srcDir("src/rider/main/kotlin")
        }
        resources {
            srcDir("src/rider/main/resources")
        }
    }
}


intellij {
    type.set("RD")
    pluginName.set("AbpInsight")
    version.set(productVersion)
    intellijRepository.set("https://cache-redirector.jetbrains.com/intellij-repository")
    // Sources aren't available for Rider
    downloadSources.set(false)
    instrumentCode.set(false)
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

    rdgen {
        val modelDir = File(rootDir, "protocol/src/main/kotlin/model")
        val csOutput = File(rootDir, "src/dotnet/AbpInsight/Protocol")
        val ktOutput = File(rootDir, "src/rider/main/kotlin/protocol")

        verbose = true


        if (!extra.has("riderSdkRoot")) {
            extra["riderSdkRoot"] = getByName(
                IntelliJPluginConstants.SETUP_DEPENDENCIES_TASK_NAME,
                SetupDependenciesTask::class
            ).idea.get().classes
        }

        classpath(File(extra["riderSdkRoot"] as File, "lib/rd/rider-model.jar"))
        sources(modelDir)
        hashFolder = File(buildDir, "rdgen").absolutePath

        packages = "model.rider"

        generator {
            language = "kotlin"
            transform = "asis"
            root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
            directory = ktOutput.canonicalPath
        }

        generator {
            language = "csharp"
            transform = "reversed"
            root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
            directory = csOutput.canonicalPath
        }


    }

    patchPluginXml {
        sinceBuild.set("243")
    }


    val buildBackend by registering {
        doLast {

            val dotnetCli = getDotnetCli()


            val file = File(backendRoot, "AbpInsight/AbpInsight.Rider.csproj")

            val buildArguments = listOf(
                "build",
                file.canonicalPath,
                "/p:Configuration=$buildConfiguration",
                "/p:Version=$version",
                "/nologo"
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

}



