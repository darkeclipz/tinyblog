﻿using System.Diagnostics;
using System.Reflection;

namespace TinyBlog;

public class TinyBlogEngine(TinyBlogSettings settings)
{
    readonly Directory InputDirectory = Directory.Create(settings.InputDirectory);
    readonly Directory OutputDirectory = Directory.Create(settings.OutputDirectory);
    readonly Template Template = Template.Create(File.Create(Path.Combine(settings.ThemesFolder, settings.Theme, settings.TemplateName)));
    readonly TinyBlogSettings Settings = TinyBlogSettings.Validate(settings);

    public void Build()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        Guard.Against.MissingThemeFolder(Settings);
        Guard.Against.MissingTheme(Settings);
        Guard.Against.MissingTemplate(Settings);
        Guard.Against.MissingStylesheet(Settings);

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
                    .SaveTo(OutputDirectory);

                Log(LogCategory.Build, file.AbsolutePath);
            });

        if (Settings.GenerateTableOfContents)
        {
            tableOfContents
                .InjectIn(Template, Settings)
                .SaveTo(OutputDirectory);

            Log(LogCategory.Info, "Generated table of contents.");
        }

        stopwatch.Stop();
        Log(LogCategory.Success, $"Build completed in {stopwatch.ElapsedMilliseconds}ms.");
    }

    public void Watch()
    {
        Guard.Against.MissingThemeFolder(Settings);
        Guard.Against.MissingTheme(Settings);
        Guard.Against.MissingTemplate(Settings);
        Guard.Against.MissingStylesheet(Settings);

        CopyStaticContent();
        CopyThemeStylesheet();

        using var watcher = new System.IO.FileSystemWatcher(Settings.InputDirectory, Filter.Markdown);
        watcher.EnableRaisingEvents = true;
        watcher.Changed += (sender, e) =>
        {
            Template template = Template.Create(File.Create(Path.Combine(Settings.ThemesFolder, Settings.Theme, Settings.TemplateName)));
            System.Threading.Thread.Sleep(250); // Wait for file to unlock, yes I know...

            Post.From(File.Create(e.FullPath))
                .GetHeaderOrDefault(Settings)
                .InsertIn(template)
                .SaveTo(OutputDirectory);

            Log(LogCategory.Build, e.FullPath);
        };

        Log(LogCategory.Watch, "Watching for changes. Press any key to exit.");
        Console.ReadKey();
    }

    public static int Init(InitOptions options)
    {
        string currentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();
        string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            ?? throw new InvalidOperationException();

        var currentDirectory = TinyBlog.Directory.Create(currentWorkingDirectory);
        var assemblyDir = TinyBlog.Directory.Create(assemblyDirectory);

        if (!options.Force)
        {
            Guard.Against.BlogAlreadyInitialized(currentDirectory);
        }

        var defaultSettings = TinyBlogSettings.CreateDefaultConfiguration(currentDirectory, options);
        Log(LogCategory.Info, "Creating default configuration file.");

        var sourceInputDirectory = TinyBlog.Directory.Create(Path.Combine(assemblyDir.AbsolutePath, defaultSettings.InputDirectory));
        var targetInputDirectory = TinyBlog.Directory.Create(Path.Combine(currentDirectory.AbsolutePath, defaultSettings.InputDirectory));
        sourceInputDirectory.CopyFilesRecursively(targetInputDirectory, replace: options.Force);
        Log(LogCategory.Copy, "Creating src directory.");

        var sourceThemes = TinyBlog.Directory.Create(Path.Combine(assemblyDir.AbsolutePath, defaultSettings.ThemesFolder));
        var targetThemes = TinyBlog.Directory.Create(Path.Combine(currentDirectory.AbsolutePath, defaultSettings.ThemesFolder));
        sourceThemes.CopyFilesRecursively(targetThemes, replace: options.Force);
        Log(LogCategory.Copy, "Creating themes directory.");

        Log(LogCategory.Success, "Initialized blog.");

        return 0;
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
                Log(LogCategory.Copy, file.AbsolutePath);
            });
    }

    private void CopyThemeStylesheet()
    {
        File stylesheet = File.Create(Path.Combine(Settings.ThemesFolder, Settings.Theme, Settings.StylesheetName));
        stylesheet.CopyTo(OutputDirectory);
        Log(LogCategory.Copy, stylesheet.AbsolutePath);
    }

    public static void Log(LogCategory category, string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"[{timestamp}] ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        if (category == LogCategory.Success)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
        }
        else if (category == LogCategory.Warning)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
        }
        else if (category == LogCategory.Error)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.Write($"{category.ToString().ToLower()}");
        Console.ResetColor();
        Console.Write(" ");
        Console.WriteLine(message);
    }
}

public enum LogCategory
{
    Build,
    Info,
    Copy,
    Success,
    Watch,
    Warning,
    Error
}