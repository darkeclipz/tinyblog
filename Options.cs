using CommandLine;

namespace TinyBlog
{
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; } = false;

        [Option('t', "template", Required = false, HelpText = "Path to the template file.")]
        public string TemplatePath { get; set; } = "layout.html";

        [Option('w', "watch", Required = false, HelpText = "Watch for changes in the articles directory.")]
        public bool Watch { get; set; } = false;

        [Option("input-dir", Required = false, HelpText = "Path to the input directory.")]
        public string InputDir { get; set; } = "articles";

        [Option("output-dir", Required = false, HelpText = "Path to the output directory.")]
        public string OutputDir { get; set; } = "static";

        [Option('s', "stylesheet", Required = false, HelpText = "Path to the stylesheet file.")]
        public string StylesheetPath { get; set; } = "default.css";
    }
}
