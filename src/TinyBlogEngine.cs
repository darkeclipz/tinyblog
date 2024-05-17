using System.Diagnostics;
using System.Reflection;

namespace TinyBlog;

public class TinyBlogEngine(TinyBlogSettings settings)
{
    private readonly Directory _inputDirectory = Directory.Create(settings.InputDirectory);
    private readonly Directory _outputDirectory = Directory.Create(settings.OutputDirectory);
    private readonly TinyBlogSettings _settings = TinyBlogSettings.Validate(settings);
    private Template _template = Template.Create(File.Create(System.IO.Path.Combine(TinyBlogSettings.ThemesFolder, settings.Theme, TinyBlogSettings.TemplateName)));

    public void Build()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        Guard.Against.MissingThemeFolder(_settings);
        Guard.Against.MissingTheme(_settings);
        Guard.Against.MissingTemplate(_settings);
        Guard.Against.MissingStylesheet(_settings);

        _outputDirectory.Clear();
        CopyStaticContent();
        CopyThemeStylesheet();

        TableOfContents tableOfContents = new();

        _inputDirectory
            .EnumerateFiles(Filter.Markdown, SearchOption.TopDirectoryOnly)
            .ForEach(file =>
            {
                Post.From(file)
                    .GetHeaderOrDefault(_settings)
                    .InsertIn(_template)
                    .AddTo(tableOfContents)
                    .SaveTo(_outputDirectory)
                    .OnSuccess(() => Logger.LogBuild(file.AbsolutePath));
            });

        if (_settings.GenerateTableOfContents)
        {
            tableOfContents
                .InsertIn(_template, _settings)
                .SaveTo(_outputDirectory);

            Logger.LogInfo("Generated table of contents.");
        }

        stopwatch.Stop();
        Logger.LogSuccess($"Build completed in {stopwatch.ElapsedMilliseconds}ms.");
    }

    public void Watch()
    {
        Build();

        using var watcher = new System.IO.FileSystemWatcher(_settings.InputDirectory, Filter.Markdown);
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;
        watcher.Changed += (sender, e) =>
        {
            _template = Template.Create(File.Create(Path.Combine(TinyBlogSettings.ThemesFolder, _settings.Theme, TinyBlogSettings.TemplateName)));
            System.Threading.Thread.Sleep(250); // Wait for file to unlock, yes I know...
            Build();
        };

        Logger.LogWatch("Watching for changes. Press any key to exit.");
        Console.ReadKey();
    }

    public static void Init(InitOptions options)
    {
        var currentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();
        var assemblyDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
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

        var sourceThemes = Directory.Create(Path.Combine(assemblyDir.AbsolutePath, TinyBlogSettings.ThemesFolder));
        var targetThemes = Directory.Create(Path.Combine(currentDirectory.AbsolutePath, TinyBlogSettings.ThemesFolder));
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
        _inputDirectory
            .EnumerateFiles(Filter.All, SearchOption.TopDirectoryOnly)
            .Where(f => f.FileName.Extension != Markdown.Extension)
            .ForEach(file =>
            {
                file.CopyTo(_outputDirectory);
                Logger.LogCopy(file.AbsolutePath);
            });
    }

    private void CopyThemeStylesheet()
    {
        var stylesheet = File.Create(Path.Combine(TinyBlogSettings.ThemesFolder, _settings.Theme, TinyBlogSettings.StylesheetName));
        stylesheet.CopyTo(_outputDirectory);
        Logger.LogCopy(stylesheet.AbsolutePath);
    }
}