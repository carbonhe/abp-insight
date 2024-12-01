rootProject.name = "AbpInsight"

pluginManagement {
    val rdVersion: String by settings
    repositories {
        if (rdVersion == "SNAPSHOT")
            mavenLocal()

        maven("https://cache-redirector.jetbrains.com/intellij-dependencies")
        maven("https://cache-redirector.jetbrains.com/plugins.gradle.org")
        maven("https://cache-redirector.jetbrains.com/maven-central")
    }
    plugins {
        id("com.jetbrains.rdgen") version rdVersion
        // I don't know why this plugin's version influence the rider() in intellijPlatform dependencies
        id("org.jetbrains.intellij.platform") version "2.0.0-beta8"
        kotlin("jvm") version "1.9.23"

    }
    resolutionStrategy {
        eachPlugin {
            when (requested.id.name) {
                // This required to correctly rd-gen plugin resolution. May be we should switch our naming to match Gradle plugin naming convention.
                "rdgen" -> {
                    useModule("com.jetbrains.rd:rd-gen:${rdVersion}")
                }
            }
        }
    }
}

include(":protocol")
