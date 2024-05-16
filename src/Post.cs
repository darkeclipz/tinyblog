using System.Text.RegularExpressions;

namespace TinyBlog;

public record Markdown(string Value)
{
    public static string Extension { get; } = "md";
}

public class PostHeader
{
    public string Title { get; private set; } = string.Empty;
    public DateTimeOffset Date { get; private set; }
    public string Author { get; private set; } = string.Empty;
    public bool IsHidden { get; private set; } = false;
    public bool IsPublished { get; private set; } = true;

    public static PostHeader Create(string title, string author, string date, string hidden, string published)
    {
        return new PostHeader
        {
            Title = title,
            Author = author,
            Date = DateTimeOffset.Parse(date),
            IsHidden = hidden == "true",
            IsPublished = published == "true"
        };
    }
}

public class Post
{
    public File File { get; private set; } = null!;
    public Markdown Markdown { get; private set; } = null!;
    public Html? Html { get; private set; }
    public PostHeader? Header { get; private set; }

    public Post InsertIn(Template template)
    {
        string post = Markdig.Markdown.ToHtml(Markdown?.Value ?? string.Empty);
        Html = template.Html
            .Replace("title", Header?.Title ?? string.Empty)
            .Replace("author", Header?.Author ?? string.Empty)
            .Replace("date", Header?.Date.ToString() ?? string.Empty)
            .Replace("content", post);
        return this;
    }

    public Post GetHeaderOrDefault(TinyBlogSettings settings)
    {
        static bool TryGetProperty(string property, string input, out string value)
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

        if (Markdown.Value.StartsWith("---"))
        {
            var end = Markdown.Value.IndexOf("---", 3);

            if (end > 0)
            {
                var header = Markdown.Value[3..end].Trim();

                if (!TryGetProperty("title", header, out string title))
                {
                    Logger.LogWarning($"{File.FileName} has no title.");
                    title = "Untitled post";
                }

                if (!TryGetProperty("author", header, out string author))
                {
                    author = settings.DefaultAuthor;
                }

                if (!TryGetProperty("date", header, out string date))
                {
                    Logger.LogWarning($"{File.FileName} has no date.");
                    date = DateTimeOffset.Now.ToString();
                }

                if (!TryGetProperty("hidden", header, out string hidden))
                {
                    hidden = "false";
                }

                if (!TryGetProperty("published", header, out string published))
                {
                    published = "true";
                }

                Header = PostHeader.Create(title, author, date, hidden, published);
            }

            Markdown = new Markdown(Markdown.Value[(end + 3)..].Trim());
        }
        else
        {
            Header = PostHeader.Create("Untitled post", settings.DefaultAuthor, DateTimeOffset.Now.ToString(), hidden: "false", published: "true");
        }

        return this;
    }

    public Post AddTo(TableOfContents tableOfContents)
    {
        tableOfContents.AddPost(this);
        return this;
    }

    public void SaveTo(Directory directory)
    {
        if (Header?.IsPublished ?? true)
        {
            directory.Save(this);
        }
    }

    public static Post From(File file)
    {
        return new Post
        {
            File = file,
            Markdown = new Markdown(file.ReadAllText())
        };
    }

    public override string ToString() => Html?.Value ?? string.Empty;
}
