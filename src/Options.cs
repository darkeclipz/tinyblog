using CommandLine;

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

[Verb("watch", HelpText = "Watch the current directory for changes and build.")]
class WatchOptions;

[Verb("version", HelpText = "Display the version.")]
class VersionOptions;