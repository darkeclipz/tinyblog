using System.Text.RegularExpressions;

namespace TinyBlog;

public record Markdown(string Value)
{
    public const string Extension = "md";
}

public static class PostHeaderParameter
{
    public const string Title = "title";
    public const string Author = "author";
    public const string Date = "date";
    public const string Hidden = "hidden";
    public const string Published = "published";
}

public class PostHeader
{
    public string Title { get; private init; } = string.Empty;
    public DateTimeOffset Date { get; private init; }
    public string Author { get; private init; } = string.Empty;
    public bool IsHidden { get; private set; }
    public bool IsPublished { get; private init; } = true;

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
    public File File { get; private init; } = null!;
    private Markdown Markdown { get; set; } = null!;
    private Html? Html { get; set; }
    public PostHeader? Header { get; private set; }

    public Post InsertIn(Template template)
    {
        var post = Markdig.Markdown.ToHtml(Markdown.Value);

        Html = template.Html
            .Replace(Placeholder.Title, Header?.Title ?? string.Empty)
            .Replace(Placeholder.Author, Header?.Author ?? string.Empty)
            .Replace(Placeholder.Date, Header?.Date.ToString() ?? string.Empty)
            .Replace(Placeholder.Content, post)
            .Replace(Placeholder.Now, DateTimeOffset.Now.ToString())
            .Replace(Placeholder.Year, DateTimeOffset.Now.Year.ToString());

        return this;
    }

    public Post GetHeaderOrDefault(TinyBlogSettings settings)
    {
        if (Markdown.Value.StartsWith("---"))
        {
            var end = Markdown.Value.IndexOf("---", 3, StringComparison.Ordinal);

            if (end > 0)
            {
                var header = Markdown.Value[3..end].Trim();

                if (!TryGetProperty(PostHeaderParameter.Title, header, out var title))
                {
                    Logger.LogWarning($"{File.FileName} has no title.");
                    title = "Untitled post";
                }

                if (!TryGetProperty(PostHeaderParameter.Author, header, out var author))
                {
                    author = settings.DefaultAuthor;
                }

                if (!TryGetProperty(PostHeaderParameter.Date, header, out var date))
                {
                    Logger.LogWarning($"{File.FileName} has no date, using current date instead.");
                    date = DateTimeOffset.Now.ToString();
                }

                if (!TryGetProperty(PostHeaderParameter.Hidden, header, out var hidden))
                {
                    hidden = "false";
                }

                if (!TryGetProperty(PostHeaderParameter.Published, header, out var published))
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
    }

    public Post AddTo(TableOfContents tableOfContents)
    {
        tableOfContents.AddPost(this);
        return this;
    }

    public Promise SaveTo(Directory directory)
    {
        if (!(Header?.IsPublished ?? true))
        {
            return Promise.Failed;
        }

        directory.Save(this);
        return Promise.Success;

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
