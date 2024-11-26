package icons

import com.intellij.openapi.util.IconLoader

// FYI: Icons are defined in C# files in the backend. When being shown in the frontend, only the icon ID is passed to
// the frontend, and IJ will look it up in resources/resharper. The name of the enclosing C# class is stripped of any
// trailing "Icons" or "ThemedIcons" and used as the folder name. The icon is named after the inner class. IJ will
// automatically add `_dark` to the basename of the SVG file if in Darcula.
// Note that IJ has a different palette and colour scheme to ReSharper. This means that the front end svg files might
// not be the same as the backed C# files...

// We need to be in the icons root package so we can use this class from plugin.xml. We also need to use @JvmField so
// that the kotlin value is visible as a JVM field via reflection
class AbpInsightIcons {

    class Icons {
        companion object {
            @JvmField
            val Logo = IconLoader.getIcon("/resharper/AbpInsight/Logo.svg", AbpInsightIcons::class.java)
        }
    }

    class Actions {
        companion object {
            @JvmField
            val AddAbpWidget = Icons.Logo
        }
    }

}

