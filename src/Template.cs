namespace TinyBlog;

public record Html(string Value)
{
    public Html Replace(string name, string value)
    {
        return new Html(Value.Replace($"{{{{ {name} }}}}", value));
    }
}

public static class Placeholder
{
    public static readonly string Title = "title";
    public static readonly string Author = "author";
    public static readonly string Date = "date";
    public static readonly string Content = "content";
    public static readonly string Now = "now";
    public static readonly string Year = "year";
    public static readonly string Active = "active";
}

public class Template
{
    public File File { get; private set; } = null!;
    public Html Html { get; private set; } = null!;

    public static Template Create(File template)
    {
        var html = System.IO.File.ReadAllText(template.AbsolutePath);

        Guard.Against.MissingRequiredPlaceholder(html, Placeholder.Title);
        Guard.Against.MissingRequiredPlaceholder(html, Placeholder.Content);
        Guard.Against.MissingOptionalPlaceholder(html, Placeholder.Author);
        Guard.Against.MissingOptionalPlaceholder(html, Placeholder.Date);

        return new Template
        {
            File = template,
            Html = new Html(html)
        };
    }
}