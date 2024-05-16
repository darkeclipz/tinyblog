namespace TinyBlog;

public static class Filter
{
    public static string All => "*.*";
    public static string Markdown => "*.md";
}

public record FileName(string Name, string Extension)
{
    public override string ToString() => $"{Name}.{Extension}";
}

public class File
{
    public FileName FileName { get; private set; } = null!;
    public string AbsolutePath { get; private set; } = string.Empty;

    public void CopyTo(Directory to)
    {
        System.IO.File.Copy(AbsolutePath, Path.Combine(to.AbsolutePath, FileName.ToString()), overwrite: true);
    }

    public string ReadAllText()
    {
        return System.IO.File.ReadAllText(AbsolutePath);
    }

    public bool Exists()
    {
        return System.IO.File.Exists(AbsolutePath);
    }

    public static File Create(string path)
    {
        return new File
        {
            FileName = new FileName(Path.GetFileNameWithoutExtension(path), Path.GetExtension(path).TrimStart('.')),
            AbsolutePath = Path.GetFullPath(path)
        };
    }
}
