using System.Diagnostics;
using TinyBlog;
using CommandLine;
using Directory = TinyBlog.Directory;

var currentDirectory = Directory.From(System.IO.Directory.GetCurrentDirectory());
var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cancellationTokenSource.Cancel();
};

try
{
    // ReSharper disable once RedundantNameQualifier
    CommandLine.Parser.Default.ParseArguments<InitOptions, BuildOptions, WatchOptions, ServeOptions>(args)
        .MapResult(
            (InitOptions options) => Init(options),
            (BuildOptions _) => Run(watch: false),
            (WatchOptions _) => Run(watch: true),
            (ServeOptions options) => Serve(options),
            _ => ExitCode.UnknownError);
}
catch (EmptyParameterException epe)
{
    Logger.LogError($"Parameter '{epe.ParameterName}' is empty");
    return (int)ExitCode.EmptyParameter;
}
catch (ThemeFolderNotFoundException)
{
    Logger.LogError("Themes folder not found.");
    return (int)ExitCode.ThemeFolderNotFound;
}
catch (ThemeNotFoundException)
{
    Logger.LogError($"Theme not found.");
    return (int)ExitCode.ThemeNotFound;
}
catch (LayoutNotFoundException)
{
    Logger.LogError($"{TinyBlogSettings.LayoutName} not found in theme folder.");
    return (int)ExitCode.LayoutNotFound;
}
catch (StylesheetNotFoundException)
{
    Logger.LogError($"{TinyBlogSettings.StylesheetName} not found in theme folder.");
    return (int)ExitCode.StylesheetNotFound;
}
catch (MissingPlaceholderException mpe)
{
    Logger.LogError($"Placeholder '{{{{ {mpe.PlaceholderName} }}}}' is missing in layout.");
    return (int)ExitCode.MissingPlaceholder;
}
catch (SettingsNotFoundException)
{
    Logger.LogWarning("No settings file found. Run 'tinyblog init' to create one.");
    return (int)ExitCode.SettingsNotFound;
}
catch (Exception ex)
{
    Logger.LogError(ex.Message);
    return (int)ExitCode.UnknownError;
}
finally
{
    cancellationTokenSource.Cancel();
}

return (int)ExitCode.Success;

ExitCode Init(InitOptions options)
{
    TinyBlogEngine.Init(options);
    
    Run(watch: false);
    
    return ExitCode.Success;
}

ExitCode Run(bool watch)
{
    var blog = TinyBlogEngine.Create(cancellationTokenSource.Token);

    if (watch)
    {
        blog.Watch();
    }
    else
    {
        blog.Build();
    }

    return ExitCode.Success;
}

ExitCode Serve(ServeOptions options)
{
    Guard.Against.MissingSettings(currentDirectory);
    var settings = TinyBlogSettings.From(currentDirectory);

    WebHost.CreateAndStart(currentDirectory.AbsolutePath, settings.OutputDirectory, options.Port);
    Logger.LogInfo($"Web server started at http://localhost:{options.Port}.");
    
    if (options.Open)
    {
        var openTableOfContents = new ProcessStartInfo
        {
            FileName = $"http://localhost:{options.Port}/index.html",
            UseShellExecute = true
        };
        
        Process.Start(openTableOfContents);
    }

    Run(watch: true);

    return ExitCode.Success;
}

internal enum ExitCode
{
    Success = 0,
    UnknownError = 1,
    ThemeFolderNotFound = 2,
    ThemeNotFound = 3,
    LayoutNotFound = 4,
    StylesheetNotFound = 5,
    MissingPlaceholder = 6,
    SettingsNotFound = 7,
    EmptyParameter = 8
}
