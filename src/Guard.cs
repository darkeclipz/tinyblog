﻿namespace TinyBlog;

public class ThemeFolderNotFoundException : Exception;

public class ThemeNotFoundException : Exception;

public class LayoutNotFoundException : Exception;

public class StylesheetNotFoundException : Exception;

public class BlogAlreadyInitializedException : Exception;

public class SettingsNotFoundException : Exception;

public class EmptyParameterException(string parameterName) : Exception
{
    public string ParameterName { get; private set; } = parameterName;
}

public class MissingPlaceholderException(string placeholderName) : Exception
{
    public string PlaceholderName { get; private set; } = placeholderName;
}

public static class Guard
{
    public static class Against
    {
        public static void MissingThemeFolder()
        {
            if (!System.IO.Directory.Exists(TinyBlogSettings.ThemesFolder))
            {
                throw new ThemeFolderNotFoundException();
            }
        }

        public static void MissingTheme(TinyBlogSettings settings)
        {
            if (!System.IO.Directory.Exists(Path.Combine(TinyBlogSettings.ThemesFolder, settings.Theme)))
            {
                throw new ThemeNotFoundException();
            }
        }

        public static void MissingLayout(TinyBlogSettings settings)
        {
            if (!System.IO.File.Exists(Path.Combine(TinyBlogSettings.ThemesFolder, settings.Theme, TinyBlogSettings.LayoutName)))
            {
                throw new LayoutNotFoundException();
            }
        }

        public static void MissingStylesheet(TinyBlogSettings settings)
        {
            if (!System.IO.File.Exists(Path.Combine(TinyBlogSettings.ThemesFolder, settings.Theme, TinyBlogSettings.StylesheetName)))
            {
                throw new StylesheetNotFoundException();
            }
        }

        public static void EmptyParameter(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new EmptyParameterException(name);
            }
        }

        public static void MissingRequiredPlaceholder(string text, string placeholderName)
        {
            if (!text.Contains($"{{{{ {placeholderName} }}}}", StringComparison.Ordinal))
            {
                throw new MissingPlaceholderException(placeholderName);
            }
        }

        public static void MissingOptionalPlaceholder(string text, string placeholderName)
        {
            if (!text.Contains($"{{{{ {placeholderName} }}}}", StringComparison.Ordinal))
            {
                Logger.LogWarning($"Placeholder '{{{{ {placeholderName} }}}}' is missing in layout.");
            }
        }

        public static void BlogAlreadyInitialized(Directory currentWorkingDirectory)
        {
            if (TinyBlogSettings.ExistsIn(currentWorkingDirectory))
            {
                throw new BlogAlreadyInitializedException();
            }
        }

        public static void MissingSettings(Directory currentWorkingDirectory)
        {
            if (!TinyBlogSettings.ExistsIn(currentWorkingDirectory))
            {
                throw new SettingsNotFoundException();
            }
        }
    }
}
