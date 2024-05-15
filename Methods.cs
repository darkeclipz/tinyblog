using Markdig;
using System.Text.RegularExpressions;
using System.Text;

namespace TinyBlog;

static class Methods
{
    internal static void Convert(string articlePath, string outputDir, Template template)
    {
        string article = ReadFileContents(articlePath);
        string title = string.Empty;
        string author = string.Empty;
        string date = string.Empty;

        if (article.StartsWith("---"))
        {
            var end = article.IndexOf("---", 3);

            if (end > 0)
            {
                var header = article[3..end].Trim();
                TryGetProperty("title", header, out title);
                TryGetProperty("author", header, out author);
                TryGetProperty("date", header, out date);
            }

            article = article[(end + 3)..].Trim();
        }
        else
        {
            title = Path.GetFileNameWithoutExtension(articlePath);
        }

        DateTime dateTime = DateTime.Now;

        if (!string.IsNullOrEmpty(date))
        {
            dateTime = DateTime.Parse(date);
        }

        string output = template.HTML
            .Replace("{{ title }}", title)
            .Replace("{{ author }}", author)
            .Replace("{{ date }}", dateTime.ToString("R"))
            .Replace("{{ content }}", Markdown.ToHtml(article));

        string fileName = title.Replace(" ", "-").ToLower() + ".html";
        string filePath = Path.Combine(outputDir, fileName);
        File.WriteAllText(filePath, output);
        Console.WriteLine($"Generated {fileName}.");
    }

    internal static string ReadFileContents(string filePath)
    {
        using FileStream fs = File.OpenRead(filePath);
        byte[] buffer = new byte[fs.Length];
        fs.Read(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer);
    }

    internal static bool TryGetProperty(string property, string input, out string value)
    {
        var match = Regex.Match(input, $"^{property}:(.*)$", RegexOptions.Multiline);
        if (match.Groups.Count != 2)
        {
            value = string.Empty;
            return false;
        }
        value = match.Groups[1].Value.Trim();
        return true;
    }
}
