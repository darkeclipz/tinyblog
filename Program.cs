using TinyBlog;
using CommandLine;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(o =>
    {
        Template template = Template.Create(o.TemplatePath);
        Directory.CreateDirectory(o.OutputDir);
        string styleSheetName = Path.GetFileName(o.StylesheetPath);

        string stylesheet = Path.Combine(o.OutputDir, styleSheetName);
        if (!File.Exists(stylesheet))
        {
            File.Copy(o.StylesheetPath, stylesheet);
        }

        if (o.Watch)
        {
            using var watcher = new FileSystemWatcher(o.InputDir, "*.md");
            watcher.EnableRaisingEvents = true;
            watcher.Changed += (sender, e) =>
            {
                Thread.Sleep(250); // Wait for file to unlock.
                Methods.Convert(e.FullPath, o.OutputDir, template);
            };

            Console.WriteLine("Watching for changes. Press any key to exit.");
            Console.ReadKey();
        }
        else
        {
            string[] articles = Directory.GetFiles(o.InputDir, "*.md");

            foreach (var articlePath in articles)
            {
                Methods.Convert(articlePath, o.OutputDir, template);
            }
        }
    });