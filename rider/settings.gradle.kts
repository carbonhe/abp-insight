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
        id("org.jetbrains.intellij.platform") version "2.2.1"
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
