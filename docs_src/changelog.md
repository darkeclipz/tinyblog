---
title: Change log
author: Lars Rotgers
date: 2024-05-16T08:52:30+0100
hidden: false
---

<img src="logo.png" style="max-width: 64px;" alt="Logo of tinyblog" />

# tinyblog
### Let's build the smallest blog engine you can imagine

---

## tinyblog-1.0.0

The initial release of tinyblog contains the following features:
 
 * Define a template in `layout.html` and the style in `default.css` that are used for the blog.
 * Write blog posts in Markdown files in the `articles` folder. tinyblog will convert these posts to HTML by using the defined layout and stylesheet.
 * A CLI tool that converts all the articles that are written with Markdown in a directory to blog posts.
 * The CLI tool has a `--watch` option that will watch a directory and convert files to blog posts upon a change.
 * Syntax highlighting is included for code blocks by highlight.js.
 * Support for mathematics by MathJax.