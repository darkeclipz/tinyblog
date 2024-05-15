namespace TinyBlog;

record Template(string HTML)
{
    public static Template Create(string templatePath) => new(File.ReadAllText(templatePath));
}