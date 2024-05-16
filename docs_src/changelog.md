---
title: Change log
author: Lars Rotgers
hidden: false
---

## tinyblog-2.0.1 (not released)

This is a list of upcoming changes that have not yet been released.

 * Removed the h1 header from the table of contents.
 * After initializing a blog, a build is always created.

## tinyblog-2.0.0

Release 2.0.0 features a complete rewrite of the blogging engine. This is done to streamline the code and provide a good baseline to add the following new functionality:

  * There is a newly defined directory structure.
  * Support for different themes.
  * A `settings.yml` file in which the blog settings and theme can be configured.
  * Added a table of contents generator.
  * Improved the CLI with a `init`, `build,` and `watch` command.
  * Improved the CLI with log messages.
  * Improved validation and exception handling with meaningful errors.
  * Usage of `DateTimeOffset` for improved publish dates.
  * All the static files in the `src` directory are now copied to the `dist` directory.
  * A post can now be `hidden`, so that it doesn't show up in the table of contents.

Initially there was the idea to also add plugin support for e.g. Mathjax and highlight.js. However, since this can be configured in the `template.html` file, there isn't really any added value by integrating this into tinyblog. If you want more templates, just add the `link` and `script` tags to the `template.html` in your theme.

A second idea was to add categories by allowing sub folders in the `src` directory. This idea has been postponed for now. If the need arises this will be implemented in a later release.

## tinyblog-1.0.0

The initial release of tinyblog contains the following features:
 
 * Define a template in `layout.html` and the style in `default.css` that are used for the blog.
 * Write blog posts in Markdown files in the `articles` folder. tinyblog will convert these posts to HTML by using the defined layout and stylesheet.
 * A CLI tool that converts all the articles that are written with Markdown in a directory to blog posts.
 * The CLI tool has a `--watch` option that will watch a directory and convert files to blog posts upon a change.
 * Syntax highlighting is included for code blocks by highlight.js.
 * Support for mathematics by MathJax.