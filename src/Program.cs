using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using TinyBlog;
using CommandLine;
using Microsoft.AspNetCore.Hosting;

var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cancellationTokenSource.Cancel();
};

// ReSharper disable once RedundantNameQualifier
CommandLine.Parser.Default.ParseArguments<InitOptions, BuildOptions, WatchOptions, ServeOptions>(args)
    .MapResult(
        (InitOptions opts) =>
        {
            TinyBlogEngine.Init(opts);
            Run(watch: false);
            return 0;
        },
        (BuildOptions _) => Run(watch: false),
        (WatchOptions _) => Run(watch: true),
        (ServeOptions options) => Serve(options),
        _ => 2);

return 0;

int Serve(ServeOptions options)
{
    var currentWorkingDirectory = TinyBlog.Directory.Create(System.IO.Directory.GetCurrentDirectory());
    
    Guard.Against.MissingSettings(currentWorkingDirectory);
    var settings = TinyBlogSettings.From(currentWorkingDirectory);
    var url = $"http://localhost:{options.Port}";
    
    var host = new WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(currentWorkingDirectory.AbsolutePath)
        .UseWebRoot(settings.OutputDirectory)
        .UseUrls(url)
        .UseStartup<Startup>()
        .Build();

    host.Start();
    Logger.LogInfo($"Web server started at {url}.");
    
    if (options.Open)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = $"{url}/index.html",
            UseShellExecute = true
        });
    }

    Run(watch: true);

    return 0;
}

int Run(bool watch)
{
    const int success = 0;
    const int error = 1;
    var currentWorkingDirectory = TinyBlog.Directory.Create(System.IO.Directory.GetCurrentDirectory());
    TinyBlogSettings? settings = null;

    try
    {
        Guard.Against.MissingSettings(currentWorkingDirectory);

        settings = TinyBlogSettings.From(currentWorkingDirectory);
        var blog = new TinyBlogEngine(settings, cancellationTokenSource.Token);

        if (watch)
        {
            blog.Watch();
        }
        else
        {
            blog.Build();
        }
    }
    catch (EmptyParameterException epe)
    {
        Logger.LogError($"Parameter '{epe.ParameterName}' is empty");
        return error;
    }
    catch (ThemeFolderNotFoundException)
    {
        Logger.LogError("Themes folder not found.");
        return error;
    }
    catch (ThemeNotFoundException)
    {
        Logger.LogError($"Theme '{settings!.Theme}' not found.");
        return error;
    }
    catch (LayoutNotFoundException)
    {
        Logger.LogError($"{TinyBlogSettings.LayoutName} not found in theme folder.");
        return error;
    }
    catch (StylesheetNotFoundException)
    {
        Logger.LogError($"{TinyBlogSettings.StylesheetName} not found in theme folder.");
        return error;
    }
    catch (MissingPlaceholderException mpe)
    {
        Logger.LogError($"Placeholder '{{{{ {mpe.PlaceholderName} }}}}' is missing in layout.");
        return error;
    }
    catch (SettingsNotFoundException)
    {
        Logger.LogWarning("No settings file found. Run 'tinyblog init' to create one.");
        return error;
    }
    catch (Exception ex)
    {
        Logger.LogError(ex.Message);
        return error;
    }
    finally
    {
        cancellationTokenSource.Cancel();
    }

    return success;
}

internal class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseStaticFiles();
    }
}