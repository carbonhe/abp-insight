plugins {
    id("com.jetbrains.rdgen")
    kotlin("jvm")
}




repositories {
    maven("https://cache-redirector.jetbrains.com/intellij-dependencies")
    maven("https://cache-redirector.jetbrains.com/maven-central")
}

dependencies {
    implementation("org.jetbrains.kotlin:kotlin-stdlib")
}

rdgen {
    val modelDir = File(rootDir, "protocol/src/main/kotlin/model")
    val csOutput = File(rootDir, "src/dotnet/AbpInsight/Protocol")
    val ktOutput = File(rootDir, "src/rider/main/kotlin/protocol")

    verbose = true


//    if (!extra.has("riderSdkRoot")) {
//        extra["riderSdkRoot"] = getByName(
//                IntelliJPluginConstants.SETUP_DEPENDENCIES_TASK_NAME, SetupDependenciesTask::class
//            ).idea.get().classes
//}

//    classpath(File(extra["riderSdkRoot"] as File, "lib/rd/rider-model.jar"))
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
