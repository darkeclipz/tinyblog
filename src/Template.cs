namespace TinyBlog;

public record Html(string Value)
{
    public Html Replace(string name, string value)
    {
        return new(Value.Replace($"{{{{ {name} }}}}", value));
    }
}

public class Template
{
    public File File { get; private set; } = null!;
    public Html Html { get; private set; } = null!;

    public static Template Create(File template)
    {
        string html = System.IO.File.ReadAllText(template.AbsolutePath);

        Guard.Against.MissingRequiredPlaceholder(html, "title");
        Guard.Against.MissingRequiredPlaceholder(html, "content");
        Guard.Against.MissingOptionalPlaceholder(html, "author");
        Guard.Against.MissingOptionalPlaceholder(html, "date");

        return new Template
        {
            File = template,
            Html = new Html(html)
        };
    }
}