---
title: My first post
author: John Doe
date: 2024-05-15 20:31
---

# My first post

## An example of using TinyBlog

A post can start with a title, an author, and a date. The content of the post is written in Markdown.
If the header is omitted, the file name is used to generate the HTML file.

## What's included?

All of the valid Markdown is converted into HTML, thanks to the `Markdig` package.

Additionally, it is possible to add code snippets, and these are highlighted using the `highlight.js` package, such as the example below:

```csharp
public class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello, world!");
    }
}
```

**Note:** You might have to add additional language support, or change the theme for `highlight.js` to work properly.

Finally, Mathjax has also been included. This allows for the rendering of mathematical equations, such as the following:

$$
\int_{-\infty}^{\infty} e^{-x^2} dx = \sqrt{\pi}
$$

It is also possible to use inline math with the \\$ sign. For example, $e^{i\pi} = -1$.

**Warning**: Because of this, a \\$ sign has to be delimited with two backslashes. This first backslash for Markdown, and the second for Mathjax.

## Simplicity first

The main motivation behind TinyBlog is to keep things simple. It is a single-file blog engine that is easy to understand and modify. It is not meant to be a full-fledged blog engine, but rather a starting point for you to create a simple blog without much of a hassle.

The generated HTML files are also very simple, and contain minimal CSS. This allows for easy customization, and the addition of your own CSS.