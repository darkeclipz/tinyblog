using System.Diagnostics;
using System.Reflection;

namespace TinyBlog;

public class TinyBlogEngine(TinyBlogSettings settings)
{
    readonly Directory InputDirectory = Directory.Create(settings.InputDirectory);
    readonly Directory OutputDirectory = Directory.Create(settings.OutputDirectory);
    readonly TinyBlogSettings Settings = TinyBlogSettings.Validate(settings);
    Template Template = Template.Create(File.Create(System.IO.Path.Combine(settings.ThemesFolder, settings.Theme, settings.TemplateName)));

    public void Build()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        Guard.Against.MissingThemeFolder(Settings);
        Guard.Against.MissingTheme(Settings);
        Guard.Against.MissingTemplate(Settings);
        Guard.Against.MissingStylesheet(Settings);

        OutputDirectory.Clear();
        CopyStaticContent();
        CopyThemeStylesheet();

        TableOfContents tableOfContents = new();

        InputDirectory
            .EnumerateFiles(Filter.Markdown, SearchOption.TopDirectoryOnly)
            .ForEach(file =>
            {
                Post.From(file)
                    .GetHeaderOrDefault(Settings)
                    .InsertIn(Template)
                    .AddTo(tableOfContents)
                    .SaveTo(OutputDirectory)
                    .OnSuccess(() => Logger.LogBuild(file.AbsolutePath));
            });

        if (Settings.GenerateTableOfContents)
        {
            tableOfContents
                .InsertIn(Template, Settings)
                .SaveTo(OutputDirectory);

            Logger.LogInfo("Generated table of contents.");
        }

        stopwatch.Stop();
        Logger.LogSuccess($"Build completed in {stopwatch.ElapsedMilliseconds}ms.");
    }

    public void Watch()
    {
        Build();

        using var watcher = new System.IO.FileSystemWatcher(Settings.InputDirectory, Filter.Markdown);
        watcher.EnableRaisingEvents = true;
        watcher.Changed += (sender, e) =>
        {
            Template = Template.Create(File.Create(Path.Combine(Settings.ThemesFolder, Settings.Theme, Settings.TemplateName)));
            System.Threading.Thread.Sleep(250); // Wait for file to unlock, yes I know...
            Build();
        };

        Logger.LogWatch("Watching for changes. Press any key to exit.");
        Console.ReadKey();
    }

    public static void Init(InitOptions options)
    {
        string currentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();
        string assemblyDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            ?? throw new InvalidOperationException();

        var currentDirectory = Directory.Create(currentWorkingDirectory);
        var assemblyDir = Directory.Create(assemblyDirectory);

        if (!options.Force)
        {
            Guard.Against.BlogAlreadyInitialized(currentDirectory);
        }

        var defaultSettings = TinyBlogSettings.CreateDefaultConfiguration(currentDirectory, options);
        Logger.LogInfo("Creating default configuration file.");

        var sourceInputDirectory = Directory.Create(Path.Combine(assemblyDir.AbsolutePath, defaultSettings.InputDirectory));
        var targetInputDirectory = Directory.Create(Path.Combine(currentDirectory.AbsolutePath, defaultSettings.InputDirectory));
        sourceInputDirectory.CopyFilesRecursively(targetInputDirectory, replace: options.Force);
        Logger.LogCopy("Creating src directory.");

        var sourceThemes = Directory.Create(Path.Combine(assemblyDir.AbsolutePath, defaultSettings.ThemesFolder));
        var targetThemes = Directory.Create(Path.Combine(currentDirectory.AbsolutePath, defaultSettings.ThemesFolder));
        sourceThemes.CopyFilesRecursively(targetThemes, replace: options.Force);
        Logger.LogCopy("Creating themes directory.");

        Logger.LogSuccess("Initialized blog.");
    }

    public static void Version()
    {
        Console.WriteLine($"v{typeof(TinyBlogEngine).Assembly.GetName().Version}");
    }

    private void CopyStaticContent()
    {
        InputDirectory
            .EnumerateFiles(Filter.All, SearchOption.TopDirectoryOnly)
            .Where(f => f.FileName.Extension != Markdown.Extension)
            .ForEach(file =>
            {
                file.CopyTo(OutputDirectory);
                Logger.LogCopy(file.AbsolutePath);
            });
    }

    private void CopyThemeStylesheet()
    {
        File stylesheet = File.Create(Path.Combine(Settings.ThemesFolder, Settings.Theme, Settings.StylesheetName));
        stylesheet.CopyTo(OutputDirectory);
        Logger.LogCopy(stylesheet.AbsolutePath);
    }
}