namespace TinyBlog;

public class Directory
{
    public string Name { get; private set; } = string.Empty;
    public string AbsolutePath { get; private init; } = string.Empty;

    public static Directory Create(string path)
    {
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }

        return new Directory
        {
            Name = Path.GetFileName(path),
            AbsolutePath = Path.GetFullPath(path)
        };
    }
    
    public static Directory Create(params string[] path)
    {
        return Create(System.IO.Path.Combine(path));
    }

    public IEnumerable<File> EnumerateFiles(string searchPattern, SearchOption searchOption)
    {
        return System.IO.Directory
            .EnumerateFiles(AbsolutePath, searchPattern, searchOption)
            .Select(File.Create);
    }

    public void CopyFilesRecursively(Directory to, bool replace)
    {
        foreach (var directoryPath in System.IO.Directory.GetDirectories(AbsolutePath, "*", SearchOption.AllDirectories))
        {
            System.IO.Directory.CreateDirectory(directoryPath.Replace(AbsolutePath, to.AbsolutePath));
        }

        foreach (var newPath in System.IO.Directory.GetFiles(AbsolutePath, "*.*", SearchOption.AllDirectories))
        {
            System.IO.File.Copy(newPath, newPath.Replace(AbsolutePath, to.AbsolutePath), replace);
        }
    }

    public void Clear()
    {
        System.IO.Directory.Delete(AbsolutePath, recursive: true);
        System.IO.Directory.CreateDirectory(AbsolutePath);
    }

    public void Save(Post post)
    {
        System.IO.File.WriteAllText(Path.Combine(AbsolutePath, post.File.FileName.Name + ".html"), post.ToString());
    }

    public void Save(TableOfContents tableOfContents)
    {
        System.IO.File.WriteAllText(Path.Combine(AbsolutePath, "index.html"), tableOfContents.ToString());
    }
}

public static class EnumerableDirectoryExtensions
{
    public static void ForEach(this IEnumerable<File> files, Action<File> func)
    {
        foreach (var file in files)
        {
            func(file);
        }
    }
}
