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
                    .SaveTo(OutputDirectory);

                Log(LogCategory.Build, file.AbsolutePath);
            });

        if (Settings.GenerateTableOfContents)
        {
            tableOfContents
                .InsertIn(Template, Settings)
                .SaveTo(OutputDirectory);

            Log(LogCategory.Info, "Generated table of contents.");
        }

        stopwatch.Stop();
        Log(LogCategory.Success, $"Build completed in {stopwatch.ElapsedMilliseconds}ms.");
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

        Log(LogCategory.Watch, "Watching for changes. Press any key to exit.");
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
        Log(LogCategory.Info, "Creating default configuration file.");

        var sourceInputDirectory = Directory.Create(Path.Combine(assemblyDir.AbsolutePath, defaultSettings.InputDirectory));
        var targetInputDirectory = Directory.Create(Path.Combine(currentDirectory.AbsolutePath, defaultSettings.InputDirectory));
        sourceInputDirectory.CopyFilesRecursively(targetInputDirectory, replace: options.Force);
        Log(LogCategory.Copy, "Creating src directory.");

        var sourceThemes = Directory.Create(Path.Combine(assemblyDir.AbsolutePath, defaultSettings.ThemesFolder));
        var targetThemes = Directory.Create(Path.Combine(currentDirectory.AbsolutePath, defaultSettings.ThemesFolder));
        sourceThemes.CopyFilesRecursively(targetThemes, replace: options.Force);
        Log(LogCategory.Copy, "Creating themes directory.");

        Log(LogCategory.Success, "Initialized blog.");
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
            Console.ForegroundColor = ConsoleColor.Green;
        }
        else if (category == LogCategory.Warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else if (category == LogCategory.Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
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