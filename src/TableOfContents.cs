namespace TinyBlog;

public class TableOfContents
{
    private readonly List<Post> _posts = [];
    private Html? _html;

    public void AddPost(Post post)
    {
        _posts.Add(post);
    }

    public TableOfContents InsertIn(Layout layout, TinyBlogSettings settings)
    {
        _html = layout.Html
            .Replace("title", "Table of contents")
            .Replace("author", settings.DefaultAuthor)
            .Replace("date", DateTimeOffset.Now.ToString())
            .Replace("content", GetHtml())
            .Replace("now", DateTimeOffset.Now.ToString())
            .Replace("year", DateTimeOffset.Now.Year.ToString());

        return this;
    }

    public void SaveTo(Directory directory)
    {
        directory.Save(this);
    }

    private string GetHtml()
    {
        var posts = _posts
            .Where(p => !p.Header!.IsHidden)
            .Where(p => p.Header!.IsPublished)
            .OrderByDescending(p => p.Header!.Date)
            .Select(p => $"<li><a href=\"{p.File.FileName.Name}.html\">{p.Header!.Title}</a></li>");

        return $"<h2>Table of contents</h2><ul>{string.Join("", posts)}</ul>";
    }

    public override string ToString() => _html?.Value ?? string.Empty;
}
