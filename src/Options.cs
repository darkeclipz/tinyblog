using CommandLine;
// ReSharper disable All

namespace TinyBlog;

[Verb("init", HelpText = "Create a new blog in the current directory.")]
public class InitOptions
{
    [Option("name", Required = false, HelpText = "The name of the blog.")]
    public string? Name { get; set; }

    [Option("author", Required = false, HelpText = "The author of the blog.")]
    public string? Author { get; set; }

    [Option("force", Required = false, HelpText = "Overwrite all files.")]
    public bool Force { get; set; }
}

[Verb("build", HelpText = "Build the blog in the current directory.")]
class BuildOptions;

[Verb("watch", HelpText = "Watch the current directory for changes and build the changed file.")]
class WatchOptions;

[Verb("serve", HelpText = "Serve the current directory through a web server and watch for changes.")]
class ServeOptions
{
    [Option("port", Required = false, HelpText = "The port to listen on.")]
    public int Port { get; set; } = 8080;

    [Option("open", Required = false, HelpText = "Open the default browser.")]
    public bool Open { get; set; } = true;
}