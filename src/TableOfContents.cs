namespace TinyBlog;

public class TableOfContents
{
    readonly List<Post> Posts = [];
    Html? Html;

    public void AddPost(Post post)
    {
        Posts.Add(post);
    }

    public TableOfContents InsertIn(Template template, TinyBlogSettings settings)
    {
        Html = template.Html
            .Replace("title", "Table of contents")
            .Replace("author", settings.DefaultAuthor)
            .Replace("date", DateTimeOffset.Now.ToString())
            .Replace("content", GetHtml(settings.BlogName));

        return this;
    }

    public void SaveTo(Directory directory)
    {
        directory.Save(this);
    }

    private string GetHtml(string blogTitle)
    {
        var posts = Posts
            .Where(p => !p.Header!.IsHidden)
            .OrderByDescending(p => p.Header!.Date)
            .Select(p => $"<li><a href=\"{p.File.FileName.Name}.html\">{p.Header!.Title}</a></li>");

        return $"<h1>{blogTitle}</h1><hr/><h2>Table of contents</h2><ul>{string.Join("", posts)}</ul>";
    }

    public override string ToString() => Html?.Value ?? string.Empty;
}
