![Logo](logo/github-logo.png)

# tinyblog

The world's smallest blog engine that packs a punch.

https://tinyblog.gg

## Table of Contents

- [Philosophy](#philosophy)
- [Features](#features)
- [Installation](#installation)
- [Releases](#releases)
- [CLI Commands](#cli-commands)
- [Directory Structure](#directory-structure)
- [Usage Examples](#usage-examples)
- [FAQ](#faq)
- [Open Bugs](#open-bugs)
- [Used Packages](#used-packages)
- [Contributions](#contributions)
- [License](#license)

## Philosophy

The philosophy behind tinyblog is simplicity and elegance while offering powerful functionality. Inspired by Hugo, tinyblog aims to be a minimalistic alternative. The goal is not to recreate Hugo but to provide an easy-to-understand and maintainable blogging engine. You should be able to get a blog up and running in less than 5 minutes.

## Features

tinyblog includes the following features:

- CLI tool
- Markdown support
- HTML support
- highlight.js
- MathJax
- Mermaid
- Support for any additional JS libraries you want to integrate

## Installation

Installing tinyblog is straightforward:

1. **Download**: Download the latest release from the [Releases](#releases) section.
2. **Extract**: Extract the downloaded file to a folder of your choice.
3. **Add to PATH**: Add the extracted directory to your `PATH` environment variable.
    - **Windows**:
        - Open Start Search, type in "env", and select "Edit the system environment variables".
        - Click the "Environment Variables" button.
        - Under "System Variables", find the `PATH` variable, select it, and click "Edit".
        - Add the path to the tinyblog folder.
    - **macOS/Linux**:
        - Open your terminal.
        - Edit your shell profile (`~/.bashrc`, `~/.zshrc`, etc.) and add `export PATH=$PATH:/path/to/tinyblog`.
        - Run `source ~/.bashrc` (or the appropriate command for your shell).
4. **Initialize Blog**: Create a new empty folder and run `tinyblog init`.
5. **Configure Blog**: Edit the `settings.yml` file to configure your blog.
6. **Build Blog**: Run `tinyblog build` to build your blog.

To uninstall, simply delete the directory.

## Releases

- [Download the latest release (windows-x64)](https://github.com/darkeclipz/tinyblog/releases/download/release-2.1.2/tinyblog-2.1.1.zip)

## CLI Commands

The tinyblog CLI supports the following commands:

1. `tinyblog init`: Initializes a new blog in the current directory.
2. `tinyblog build`: Builds the blog in the current directory.
3. `tinyblog watch`: Watches the current directory for changes and builds the changed files.

**Note:** Theme changes require restarting the watch process.

## Directory Structure

The directory structure for a blog is as follows:

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
        ├── layout.html
        └── style.css
```

- `settings.yml`: Defines the settings and helps tinyblog recognize the folder.
- `src`: Contains source files, copied to the `dist` folder.
- Files with the `.md` extension are converted to `.html`.
- `layout.html`: Used for the given theme.
- `style.css`: Copied from the theme folder to the `includes` folder.

## Usage Examples

### Sample `settings.yml`

```yaml
blogName: My Favorite Blog
inputDirectory: src
outputDirectory: dist
theme: default
defaultAuthor: John Doe
generateTableOfContents: true
```

### Sample Markdown Post

Create a file `src/post1.md`:

```markdown
---
title: About
date: 2021-09-01 12:00:00 +0000
author: Jane Doe
hidden: false
published: true
---

# My first post

This is my first blog post!
```

Run `tinyblog build` to see the output in the `dist` folder.

Note that:
 
 * If the `author` field is left out the default author is used as defined in the settings.
 * If the `date` field is left out it defaults to the current date and time.
 * If the `hidden` field is set to `true`, the post is not included in the table of contents.
 * If the `published` field is set to `false` the post is not saved in the `dist` folder.

### Creating a new theme

To create a new theme:

 1. Create a new folder in the `themes` directory.
 2. Add a `layout.html` file.
 3. Add a `style.css` file.
 4. Change the `theme` field in the `settings.yml` file to the new theme name.
 5. Run `tinyblog build` to see the changes.

### Layout placeholders

The following placeholders can be used in the `layout.html` file:

- `{{ title }}`: The title of the post.
- `{{ author }}`: The author of the post.
- `{{ date }}`: The date of the post.
- `{{ content }}`: The content of the post. (Required)
- `{{ now }}`: The current date time.
- `{{ year}}`: The current year.

## FAQ

 1. How do I create my own index page? Disable the table of contents in the `settings.yml` and create a file `index.md`.

## Open Bugs

There are no open bugs at the moment.

## Used Packages

Tinyblog is made possible by the following open source projects:

- **Markdig**: For parsing and converting Markdown files to HTML.
- **CommandLineParser**: For creating a user-friendly CLI with commands and options.
- **highlight.js**: For syntax highlighting in code blocks.
- **MathJax**: For rendering mathematical equations.
- **Mermaid**: For creating diagrams.
- **YamlDotNet**: For managing settings.

## Contributions

Feel free to create a pull request. 😊

### Contribution Guidelines

- Fork the repository.
- Create a new branch (`git checkout -b feature-branch`).
- Commit your changes (`git commit -am 'Add new feature'`).
- Push to the branch (`git push origin feature-branch`).
- Create a new Pull Request.

## License

TBD.
