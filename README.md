![Logo](logo/github-logo.png)

This is a really really tiny blog, that is generated from Markdown files.

## Usage

Create a directory in which you want your blog, and create a folder `articles` in there. Drop the executable in the folder, or add it to your path.

### Commands

```
Copyright (C) 2024 TinyBlog

  -v, --verbose       Set output to verbose messages.

  -t, --template      Path to the template file.

  -w, --watch         Watch for changes in the articles directory.

  --input-dir         Path to the input directory.

  --output-dir        Path to the output directory.

  -s, --stylesheet    Path to the stylesheet file.

  --help              Display this help screen.

  --version           Display version information.
```

## Defaults

 * The default input directory is `articles`, any `*.md` files are converted into HTML.
 * The default output directory is `static`.
 * The default template that is used is `layout.html`.
 * The default stylesheet that is used is `default.css`.
