package com.carbon.settings

import com.intellij.openapi.options.Configurable
import com.jetbrains.rider.settings.simple.SimpleOptionsPage

class AbpInsightTemplatesOptionPage
    : SimpleOptionsPage(PAGE_NAME, "RiderAbpInsightFileTemplatesSettings"), Configurable.NoScroll {

    companion object {
        const val PAGE_NAME = "Abp Insight"
    }

    override fun getId(): String {
        return "RiderAbpInsightFileTemplatesSettings"
    }

    override fun getHelpTopic(): String {
        return "Abp Insight Help Topic"
    }
}
