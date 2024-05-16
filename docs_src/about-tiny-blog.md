---
title: Let's build the smallest blog engine you can imagine
author: Lars Rotgers
hidden: false
---

## Philosophy behind tinyblog

The philosophy behind tinyblog is that it should be as simple and elegant as possible, but yet provide capable functionalities.

This project is mainly inspired by Hugo. However, after being rather frustrated with how advanced Hugo can be, I opted to create a barebone version of it. It is not goal to recreate Hugo! The **goal is to create a blogging engine that is easy to understand and simple to maintain**. You should be able to understand how it works and get a blog up and running in less than 5 minutes.

## Installation

Installing tinyblog is as easy as following the steps below.

 1. Download the latest release and extract this into a folder somewhere. 
 2. Add that directory to your `PATH` environment variable.
 3. Create a new empty folder and run `tinyblog init` .
 4. Configure your blog in the `settings.yml` file.
 5. Build the blog with `tinyblog build`.
   
Delete the directory to uninstall it.

## CLI commands

The tinyblog CLI supports the following commands:

 1. `tinyblog init` initializes a new blog in the current directory.
 2. `tinyblog build` builds the blog in the current directory.
 3. `tinyblog watch` watches the current directory for any changes and builds the change file.

**Warning:** Theme changes require a restart of the watch process.

## Directory structure

The directory structure of a blog is defined in the following way:

```text
$ root
├── settings.yml
├── src
│   └── post1.md
├── dist
│   ├── index.html
│   └── post1.html
└── themes
    └── default
        ├── template.html
        └── style.css
```

 * The `settings.yml` file defines the settings, and helps tinyblog understand that this folder is a tinyblog folder.
 * All the files from `src` are copied to the `dist` folder.
 * All files with the `.md` extensions are converted to `.html`.
 * The `template.html` is used for the given theme.
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

tinyblog automatically converts a code block to a `pre` block with the correct class for highlight.js. Thanks Markdig!

## MathJax support

MathJax has also been included. This allows for the rendering of mathematical equations, such as the following:

$$
\int_{-\infty}^{\infty} e^{-x^2} dx = \sqrt{\pi}
$$

It is also possible to use inline math with the \\$ sign. For example, $e^{i\pi} = -1$.

## Mermaid support

It is also possible to include Mermaid diagrams if the module has been imported in the `template.html` file.

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

Cool, huh? This is actually possible because of the HTML support.

## Including HTML fragments

We still need to research if this is a good idea. In my opinion it will defeat the purpose of being a simple blog engine.
Additionally it is cool to have a Hugo like syntax where you can for example, select the top 5 blog posts and generate a list of links based on attributes of a post. However, in this case, one might just hop over to Hugo instead of using blog engine. Don't try to be something you aren't. And we aren't building Hugo, we are building a super simple blog engine.

## Open bugs

  1. Changes to `template.html` are not reflected without restarting the application while in `watch` mode.

## Used packages

TinyBlog would not have been possible without the following open source projects:
 
 * **Markdig** for parsing Markdown files and converting them to HTML.
 * **CommandLineParser** for generating an easy CLI tool with commands and options.
 * **highlight.js** for syntax highlighting in code blocks.
 * **MathJax** for formatting and displaying mathematical equations.
 * **Mermaid** for formatting and displaying diagrams.
 * **YamlDotNet** for settings management.

## Constributions

Feel free to create a pull request. 😊

## Licence

TBD.