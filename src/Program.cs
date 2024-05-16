using TinyBlog;
using CommandLine;

CommandLine.Parser.Default.ParseArguments<InitOptions, BuildOptions, WatchOptions>(args)
    .MapResult(
        (InitOptions opts) =>
        {
            TinyBlogEngine.Init(opts);
            Run(watch: false);
            return 0;
        },
        (BuildOptions opts) => Run(watch: false),
        (WatchOptions opts) => Run(watch: true),
        errs => 2);

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
        TinyBlogEngine.Log(LogCategory.Error, $"Parameter '{epe.ParameterName}' is empty");
        return error;
    }
    catch (ThemeFolderNotFoundException)
    {
        TinyBlogEngine.Log(LogCategory.Error, "Themes folder not found.");
        return error;
    }
    catch (ThemeNotFoundException)
    {
        TinyBlogEngine.Log(LogCategory.Error, $"Theme '{settings!.Theme}' not found.");
        return error;
    }
    catch (TemplateNotFoundException)
    {
        TinyBlogEngine.Log(LogCategory.Error, $"{settings!.TemplateName} not found in theme folder.");
        return error;
    }
    catch (StylesheetNotFoundException)
    {
        TinyBlogEngine.Log(LogCategory.Error, $"{settings!.StylesheetName} not found in theme folder.");
        return error;
    }
    catch (MissingPlaceholderException mpe)
    {
        TinyBlogEngine.Log(LogCategory.Error, $"Placeholder '{{{{ {mpe.PlaceholderName} }}}}' is missing in template.");
        return error;
    }
    catch (SettingsNotFoundException)
    {
        TinyBlogEngine.Log(LogCategory.Warning, "No settings file found. Run 'tinyblog init' to create one.");
        return error;
    }
    catch (Exception ex)
    {
        TinyBlogEngine.Log(LogCategory.Error, ex.Message);
        return error;
    }

    return success;
}
