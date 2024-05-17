using System.Diagnostics;

namespace TinyBlog;

public class TinyBlogEngine(TinyBlogSettings settings)
{
    private readonly Directory _inputDirectory = Directory.Create(settings.InputDirectory);
    private readonly Directory _outputDirectory = Directory.Create(settings.OutputDirectory);
    private readonly TinyBlogSettings _settings = TinyBlogSettings.Validate(settings);

    public void Build()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        Guard.Against.MissingThemeFolder();
        Guard.Against.MissingTheme(_settings);
        Guard.Against.MissingLayout(_settings);
        Guard.Against.MissingStylesheet(_settings);

        _outputDirectory.Clear();
        CopyStaticContent();
        CopyThemeStylesheet();

        var layout = Layout.Create(
            File.Create(TinyBlogSettings.ThemesFolder, settings.Theme, TinyBlogSettings.LayoutName));
        var tableOfContents = new TableOfContents();

        _inputDirectory
            .EnumerateFiles(Filter.Markdown, SearchOption.TopDirectoryOnly)
            .ForEach(file =>
            {
                Post.From(file)
                    .GetHeaderOrDefault(_settings)
                    .InsertIn(layout)
                    .AddTo(tableOfContents)
                    .SaveTo(_outputDirectory)
                    .OnSuccess(() => Logger.LogBuild(file.AbsolutePath));
            });

        if (_settings.GenerateTableOfContents)
        {
            tableOfContents
                .InsertIn(layout, _settings)
                .SaveTo(_outputDirectory);

            Logger.LogInfo("Generated table of contents.");
        }

        stopwatch.Stop();
        Logger.LogSuccess($"Build completed in {stopwatch.ElapsedMilliseconds}ms.");
    }

    public void Watch()
    {
        Build();

        using var watcher = new FileSystemWatcher(Environment.CurrentDirectory, Filter.Markdown);
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cancellationTokenSource.Cancel();
        };

        try
        {
            watcher.Changed += OnFileChanged;
            Logger.LogWatch("Watching for changes. Press Ctrl+C to exit.");
            
            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            }
        }
        finally
        {
            watcher.Changed -= OnFileChanged;
        }
    }
    
    private void OnFileChanged(object source, FileSystemEventArgs e)
    {
        Thread.Sleep(250); // Wait for file to unlock, yes I know...
        Build();
    }

    public static void Init(InitOptions options)
    {
        var currentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();
        var assemblyDirectory = AppContext.BaseDirectory;

        var currentDirectory = Directory.Create(currentWorkingDirectory);
        var assemblyDir = Directory.Create(assemblyDirectory);

        if (!options.Force)
        {
            Guard.Against.BlogAlreadyInitialized(currentDirectory);
        }

        var defaultSettings = TinyBlogSettings.CreateDefaultConfiguration(currentDirectory, options);
        Logger.LogInfo("Creating default configuration file.");

        var sourceInputDirectory = Directory.Create(assemblyDir.AbsolutePath, defaultSettings.InputDirectory);
        var targetInputDirectory = Directory.Create(currentDirectory.AbsolutePath, defaultSettings.InputDirectory);
        sourceInputDirectory.CopyFilesRecursively(targetInputDirectory, replace: options.Force);
        Logger.LogCopy("Creating src directory.");

        var sourceThemes = Directory.Create(assemblyDir.AbsolutePath, TinyBlogSettings.ThemesFolder);
        var targetThemes = Directory.Create(currentDirectory.AbsolutePath, TinyBlogSettings.ThemesFolder);
        sourceThemes.CopyFilesRecursively(targetThemes, replace: options.Force);
        Logger.LogCopy("Creating themes directory.");

        Logger.LogSuccess("Initialized blog.");
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
        var stylesheet = File.Create(TinyBlogSettings.ThemesFolder, _settings.Theme, TinyBlogSettings.StylesheetName);
        stylesheet.CopyTo(_outputDirectory);
        Logger.LogCopy(stylesheet.AbsolutePath);
    }
}