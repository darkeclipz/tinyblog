using TinyBlog;
using CommandLine;

// ReSharper disable once RedundantNameQualifier
CommandLine.Parser.Default.ParseArguments<InitOptions, BuildOptions, WatchOptions>(args)
    .MapResult(
        (InitOptions opts) =>
        {
            TinyBlogEngine.Init(opts);
            Run(watch: false);
            return 0;
        },
        (BuildOptions _) => Run(watch: false),
        (WatchOptions _) => Run(watch: true),
        _ => 2);

return 0;

static int Run(bool watch)
{
    const int success = 0;
    const int error = 1;
    var currentWorkingDirectory = TinyBlog.Directory.Create(System.IO.Directory.GetCurrentDirectory());
    TinyBlogSettings? settings = null;

    try
    {
        Guard.Against.MissingSettings(currentWorkingDirectory);

        settings = TinyBlogSettings.From(currentWorkingDirectory);
        var blog = new TinyBlogEngine(settings);

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
    catch (TemplateNotFoundException)
    {
        Logger.LogError($"{TinyBlogSettings.TemplateName} not found in theme folder.");
        return error;
    }
    catch (StylesheetNotFoundException)
    {
        Logger.LogError($"{TinyBlogSettings.StylesheetName} not found in theme folder.");
        return error;
    }
    catch (MissingPlaceholderException mpe)
    {
        Logger.LogError($"Placeholder '{{{{ {mpe.PlaceholderName} }}}}' is missing in template.");
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

    return success;
}
