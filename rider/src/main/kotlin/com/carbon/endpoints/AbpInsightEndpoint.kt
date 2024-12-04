package com.carbon.endpoints

import com.intellij.microservices.endpoints.FrameworkPresentation
import com.intellij.openapi.components.Service
import com.intellij.openapi.project.Project
import com.jetbrains.rider.microservices.endpointsProviders.RiderHttpEndpointsProvider
import com.jetbrains.rider.microservices.endpointsProviders.WebFrameworkChecker
import icons.AbpInsightIcons

class AbpInsightEndpointsProvider : RiderHttpEndpointsProvider() {
    override val endpointsProviderName: String = "AbpInsightEndpoints"

    override fun getWebFrameworkChecker(project: Project): WebFrameworkChecker {
        return project.getService(AbpInsightEndpointsPresenceChecker::class.java)
    }

    override val presentation: FrameworkPresentation = FrameworkPresentation(
        "Abp", "Abp", AbpInsightIcons.Icons.Logo
    )
}

@Service(Service.Level.PROJECT)
class AbpInsightEndpointsPresenceChecker(project: Project) : WebFrameworkChecker(project, setOf("Volo.Abp.Core"))
