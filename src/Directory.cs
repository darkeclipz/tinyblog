using System.IO;

namespace TinyBlog;

public class Directory
{
    public string Name { get; private set; } = string.Empty;
    public string AbsolutePath { get; private set; } = string.Empty;

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

    public IEnumerable<File> EnumerateFiles(string searchPattern, SearchOption searchOption)
    {
        return System.IO.Directory
            .EnumerateFiles(AbsolutePath, searchPattern, searchOption)
            .Select(File.Create);
    }

    public IEnumerable<Directory> EnumerateDirectories(string searchPattern, SearchOption searchOption)
    {
        return System.IO.Directory
            .EnumerateDirectories(AbsolutePath, searchPattern, searchOption)
            .Select(Directory.Create);
    }

    public void CopyFilesRecursively(Directory to, bool replace)
    {
        foreach (string directoryPath in System.IO.Directory.GetDirectories(AbsolutePath, "*", SearchOption.AllDirectories))
        {
            System.IO.Directory.CreateDirectory(directoryPath.Replace(AbsolutePath, to.AbsolutePath));
        }

        foreach (string newPath in System.IO.Directory.GetFiles(AbsolutePath, "*.*", SearchOption.AllDirectories))
        {
            System.IO.File.Copy(newPath, newPath.Replace(AbsolutePath, to.AbsolutePath), replace);
        }
    }

    public void Clear()
    {
        System.IO.Directory.Delete(AbsolutePath, true);
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

public static class IEnumerableDirectoryExtensions
{
    public static void ForEach(this IEnumerable<File> files, Action<File> func)
    {
        foreach (var file in files)
        {
            func(file);
        }
    }

    public static void Do(this IEnumerable<Directory> directories, Action<Directory> func)
    {
        foreach (var directory in directories)
        {
            func(directory);
        }
    }
}
