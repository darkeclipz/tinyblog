using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TinyBlog;

public class TinyBlogSettings
{
    public string BlogName { get; set; } = "Untitled blog";
    public string InputDirectory { get; set; } = "src";
    public string OutputDirectory { get; set; } = "dist";
    public string Theme { get; set; } = "default";
    public string DefaultAuthor { get; set; } = "Anonymous";
    public bool GenerateTableOfContents { get; set; } = true;

    public static TinyBlogSettings From(Directory directory)
    {
        File settingsFile = File.Create(Path.Combine(directory.AbsolutePath, SettingsFileName));
        string yml = settingsFile.ReadAllText();

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<TinyBlogSettings>(yml);
    }

    public static bool ExistsIn(Directory directory)
    {
        return System.IO.File.Exists(Path.Combine(directory.AbsolutePath, SettingsFileName));
    }
    
    public static TinyBlogSettings CreateDefaultConfiguration(Directory directory, InitOptions options)
    {
        var settings = new TinyBlogSettings();

        if (!string.IsNullOrEmpty(options.Name))
        {
            settings.BlogName = options.Name;
        }

        if (!string.IsNullOrEmpty(options.Author))
        {
            settings.DefaultAuthor = options.Author;
        }

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        System.IO.File.WriteAllText(Path.Combine(directory.AbsolutePath, SettingsFileName), 
            serializer.Serialize(settings));

        return settings;
    }

    public static TinyBlogSettings Validate(TinyBlogSettings settings)
    {
        Guard.Against.EmptyParameter(nameof(settings.BlogName), settings.BlogName);
        Guard.Against.EmptyParameter(nameof(settings.InputDirectory), settings.InputDirectory);
        Guard.Against.EmptyParameter(nameof(settings.OutputDirectory), settings.OutputDirectory);
        Guard.Against.EmptyParameter(nameof(settings.Theme), settings.Theme);
        Guard.Against.EmptyParameter(nameof(settings.DefaultAuthor), settings.DefaultAuthor);
        return settings;
    }

    [YamlIgnore] 
    public readonly string ThemesFolder = "themes";

    [YamlIgnore] 
    public readonly string TemplateName = "template.html";

    [YamlIgnore] 
    public readonly string StylesheetName = "style.css";

    static readonly string SettingsFileName = "settings.yml";
}
