using System.Text.RegularExpressions;

namespace TinyBlog;

public record Markdown(string Value)
{
    public static string Extension { get; } = "md";
}

public static class PostHeaderParameter
{
    public static readonly string Title = "title";
    public static readonly string Author = "author";
    public static readonly string Date = "date";
    public static readonly string Hidden = "hidden";
    public static readonly string Published = "published";
    public static readonly string Active = "active";
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
            .Replace(Placeholder.Title, Header?.Title ?? string.Empty)
            .Replace(Placeholder.Author, Header?.Author ?? string.Empty)
            .Replace(Placeholder.Date, Header?.Date.ToString() ?? string.Empty)
            .Replace(Placeholder.Content, post)
            .Replace(Placeholder.Now, DateTimeOffset.Now.ToString())
            .Replace(Placeholder.Year, DateTimeOffset.Now.Year.ToString())
            .Replace(Placeholder.Active, "active");

        var activeMatches = Regex.Match(template.Html.Value, @$"{{ isActive ({File.FileName.Name}) }}");
        if (activeMatches.Groups.Count == 1)
        {
            Html = Html.Replace(activeMatches.Groups[0].Value, "active");
        }

        var allMatches = Regex.Match(template.Html.Value, @"{{ isActive (\w+) }}");
        if (allMatches.Groups.Count == 2)
        {
            Html = Html.Replace(activeMatches.Groups[0].Value, string.Empty);
        }

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

                if (!TryGetProperty(PostHeaderParameter.Title, header, out string title))
                {
                    Logger.LogWarning($"{File.FileName} has no title.");
                    title = "Untitled post";
                }

                if (!TryGetProperty(PostHeaderParameter.Author, header, out string author))
                {
                    author = settings.DefaultAuthor;
                }

                if (!TryGetProperty(PostHeaderParameter.Date, header, out string date))
                {
                    Logger.LogWarning($"{File.FileName} has no date, using current date instead.");
                    date = DateTimeOffset.Now.ToString();
                }

                if (!TryGetProperty(PostHeaderParameter.Hidden, header, out string hidden))
                {
                    hidden = "false";
                }

                if (!TryGetProperty(PostHeaderParameter.Published, header, out string published))
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

    public Promise SaveTo(Directory directory)
    {
        if (Header?.IsPublished ?? true)
        {
            directory.Save(this);
            return Promise.Success;
        }

        return Promise.Failed;
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
