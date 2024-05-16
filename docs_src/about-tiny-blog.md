---
title: Let's build the smallest blog engine you can imagine
author: Lars Rotgers
date: 2024-05-16T08:52:30+0100
hidden: false
---

<img src="logo.png" style="max-width: 64px;" alt="Logo of tinyblog" />

# tinyblog
### Let's build the smallest blog engine you can imagine

---

## Philosophy

The philosophy behind tinyblog is that it should be as simple and elegant as possible, but yet provide capable functionalities.

This project is mainly inspired by Hugo. However, after being rather frustrated with how advanced Hugo can be, I opted to create a barebone version of it. It is not goal to recreate Hugo! The **goal is to create a blogging engine that is easy to understand and simple to maintain**. You should be able to understand how it works and get a blog up and running in less than 5 minutes.

## Upcoming features

  1. Use the file name of the `.md` file for the `.html` file. This ensures that the file name doesn't change if the title of the post is changed.
  2. Parse the date of a post properly with `DateTimeOffset`. Currently the time zone is missing.
  3. Add a `blog.yml` settings file, that will contain default values and settings for the blog.
     1.  `blogName`, `defaultAuthor`, `includeMathJax`, `includeHighlightJs`, `...`.
  4. Add commands to the console application, such as:
     1. `init`: initialize a directory for a blog, by adding the `layout.html`, `default.css` files and generating the required directory, as well as an example post to get started.
     2. `build`: build the blog from the Markdown files into HTML files.
     3. `<default>`: return the help information.
  5. Improved exception handling to give meaningful error messages to the user, and how the can be resolved.
  6. Add a time stamp to the logging output in the console.
  7. Make the plugins modular with a `{{ script }}` section.
  8. Template validation; give warnings if some required placeholders are missing, such as the `script` tag.
  9. Allow the creation of categories by creating folders and sub folders in the articles directory.
  10. Implement support for images and other auxiliary files that need to be copied to the output directory. If for example, an image is included in a Markdown file, the relative path to the image must be the same for the output HTML file. Perhaps copy everything that is in the `articles` folder.
  11. Allow a post to be `hidden`, which means that it will not be included in a table of contents file. This enables a user to create *hidden pages*, such as a home, about me, and contact page. In the layout itself can then be a menu, and the pages that can be accessed from the menu, are not included in the blog itself. This allows a user to create a simple website with the blog engine.
  12. Implement themes. A theme consists of a `layout.html` and `theme.css` file.
  13. Format the HTML output (this can be done with the C# XML parser):
      1.  `System.Xml.Linq.XElement.Parse(html).ToString()`.

## Directory structure

The directory structure of a blog is defined in the following way:

```
$ root
├── blog.yml
├── src
│   ├── static
│   │   └── image1.png
│   └── post1.md
├── dist
│   ├── includes
│   │   └── style.css
│   ├── static
│   │   └── image1.png
│   ├── index.html
│   └── post1.html
└── themes
    └── default
        ├── layout.html
        └── style.css
```

 * The `blog.yml` file defines the settings, and helps tinyblog understand that this folder is a tinyblog folder.
 * All the files from `src` are copied to the `dist` folder.
 * All files with the `.md` extensions are converted to `.html`.
 * It is not allowed to have a folder called `includes` in the `src` folder.
 * The `layout.html` is used for the given theme.
 * The `style.css` is copied from the theme folder into the `includes` folder.

## HTML support

Thanks to the fantastic Markdig package, all the HTML that is within a Markdown file, is correctly added to the HTML file as well.
This seemingly small feature, has a huge impact on the project.
It enables users to color outside of the lines that the blog engine provides them, and gives them the option to do whatever they want by using HTML and JS.

One of these examples is the button that we have included here:

<button id="button1">Click me!</button>
<script>
    document.getElementById("button1").addEventListener("click", () => alert("Pretty cool, huh!?"));
</script>

## highlight.js support

highlight.js has been included to improve the readability of code. Note that by default no extra languages have been included, and the `github` theme is used. The `settings.yml` file should include a few properties that allow a user to specify extra languages that need to be included, and to switch the theme more easily.

```csharp
static int GCD(int num1, int num2)
{
    int remainder;

    while (num2 != 0)
    {
        remainder = num1 % num2;
        num1 = num2;
        num2 = remainder;
    }

    return num1;
}
```

## MathJax support

MathJax has also been included. This allows for the rendering of mathematical equations, such as the following:

$$
\int_{-\infty}^{\infty} e^{-x^2} dx = \sqrt{\pi}
$$

It is also possible to use inline math with the \\$ sign. For example, $e^{i\pi} = -1$.

## Mermaid support

```mermaid
graph TD;
    A-->B;
    A-->C;
    B-->D;
    C-->D;
```

<pre class="mermaid">
graph TD
    A[Enter Chart Definition] --> B(Preview)
    B --> C{decide}
    C --> D[Keep]
    C --> E[Edit Definition]
    E --> B
    D --> F[Save Image and Code]
    F --> B
</pre>

<script type="module">
    import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
    mermaid.initialize({ startOnLoad: true });
</script>

## Including HTML fragments

Research if this is a good idea. In my opinion it will defeat the purpose of being a simple blog engine.
Additionally it is cool to have a Hugo like syntax where you can for example, select the top 5 blog posts and generate a list of links based on attributes of a post. However, in this case, one might just hop over to Hugo instead of using blog engine. Don't try to be something you aren't. And we aren't building Hugo, we are building a super simple blog engine.

## Conversion pipeline

<pre class="mermaid">
graph TD
    CreateDirectories --> CopyStylesheets --> CopyStaticContent
    ReadTemplate --> AppendLinks --> AppendScripts
    ReadPost --> ReadHeader --> ReplacePlaceholders --> SaveToFile
</pre>

## Open bugs

  1. Symbols are not correctly removed from the blog post title when it is converted into a file name. This is due to not using the file name for the output file. (TODO-1).
  2. Changes to `layout.html` are not reflected without restarting the application while in `--watch` mode.

## Used

TinyBlog would not have been possible without the following open source projects:
 
 * **Markdig**; for parsing Markdown files and converting them to HTML.
 * **CommandLineParser**; for generating an easy CLI tool with commands and options.
 * **highlight.js**; for syntax highlighting in code blocks.
 * **MathJax**; for formatting and displaying mathematical equations.
 * **Mermaid**; for formatting and displaying diagrams.
