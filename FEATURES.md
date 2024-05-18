# Possible New Features

## Menu

Possibility to define a menu in the `settings.yml` which is then available with the {{ menu }} placeholder, and rendered as the following HTML fragment:

```html
<ul>
    <li>
        <a href="#link1">Link 1</a>
    </li>
    <li>
        <a href="#link2">Link 2</a>
    </li>
    <li>
        <a href="#link3">Link 3</a>
    </li>
</ul>
```

The user can then style the menu to their likings using CSS.

> A user can just add a menu in `layout.html`, this isn't needed.

## Extended placeholders

List list of possible place holders is:

- `{{.Title}}`: The title of the post.
- `{{.Author}}`: The author of the post.
- `{{.Date}}`: The date of the post.
- `{{.Content}}`: The content of the post.
- `{{.TableOfContents}}`: The table of contents for the post.
- `{{.BlogName}}`: The name of the blog.
- `{{.Posts}}`: The list of posts.
- `{{.PostCount}}`: The number of posts.
- `{{.Theme}}`: The name of the theme.
- `{{.Style}}`: The path to the CSS file.
- `{{.Script}}`: The path to the JS file.
- `{{.MathJax}}`: The MathJax script.
- `{{.Mermaid}}`: The Mermaid script.
- `{{.Highlight}}`: The highlight.js script.
- `{{.Settings}}`: The settings for the blog.
- `{{.Post}}`: The current post.
- `{{.PostIndex}}`: The index of the current post.
- `{{.PostCount}}`: The total number of posts.
- `{{.PostTitle}}`: The title of the current post.
- `{{.PostAuthor}}`: The author of the current post.
- `{{.PostDate}}`: The date of the current post.
- `{{.PostContent}}`: The content of the current post.
- `{{.PostTableOfContents}}`: The table of contents for the current post.
- `{{.PostHidden}}`: Whether the current post is hidden.
- `{{.PostPublished}}`: Whether the current post is published.
- `{{.PostPath}}`: The path to the current post.
- `{{.PostUrl}}`: The URL of the current post.
- `{{.PostPrevious}}`: The previous post.
- `{{.PostNext}}`: The next post.
- `{{.PostPreviousUrl}}`: The URL of the previous post.
- `{{.PostNextUrl}}`: The URL of the next post.
- `{{.PostPreviousTitle}}`: The title of the previous post.
- `{{.PostNextTitle}}`: The title of the next post.
- `{{.PostPreviousAuthor}}`: The author of the previous post.
- `{{.PostNextAuthor}}`: The author of the next post.
- `{{.PostPreviousDate}}`: The date of the previous post.
- `{{.PostNextDate}}`: The date of the next post.
- `{{.PostPreviousPath}}`: The path to the previous post.
- `{{.PostNextPath}}`: The path to the next post.
- `{{.PostPreviousHidden}}`: Whether the previous post is hidden.
- `{{.PostNextHidden}}`: Whether the next post is hidden.
- `{{.PostPreviousPublished}}`: Whether the previous post is published.
- `{{.PostNextPublished}}`: Whether the next post is published.
- `{{.PostPreviousIndex}}`: The index of the previous post.
- `{{.PostNextIndex}}`: The index of the next post.
- `{{.PostPreviousTableOfContents}}`: The table of contents for the previous post.
- `{{.PostNextTableOfContents}}`: The table of contents for the next post.

## A `new` command that creates files

> Not needed, just create a new file yourself you lazy ....

## Includes

Allow the inclusion of partial markdown fragments (note that a markdown file can also just be HTML).

{{ include ___.md }}

or just read HTML from the `inputDirectory`:

{{ include ___.html }}

### Include arguments

Add arguments to inclusions.

```md
<div class="badge">
    {{ param1 }}
</div>
```

```md
{{ include badge.md "Hello world!" }}
```

Is this really necessary???