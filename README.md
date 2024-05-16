![Logo](logo/github-logo.png)

This is a really really tiny blog, that is generated from Markdown files.

## Philosophy behind tinyblog

The philosophy behind tinyblog is that it should be as simple and elegant as possible, but yet provide capable functionalities.

This project is mainly inspired by Hugo. However, after being rather frustrated with how advanced Hugo can be, I opted to create a barebone version of it. It is not goal to recreate Hugo! The **goal is to create a blogging engine that is easy to understand and simple to maintain**. You should be able to understand how it works and get a blog up and running in less than 5 minutes.

## Features

tinyblogs has the following features.

 * Markdown support
 * HMTL support
 * highlight.js
 * MathJax
 * Mermaid
 * CLI tool

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
