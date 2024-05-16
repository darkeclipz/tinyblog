namespace TinyBlog;

public static class Logger
{
    public static void LogBuild(string message) => Log(LogCategory.Build, message);
    public static void LogInfo(string message) => Log(LogCategory.Info, message);
    public static void LogCopy(string message) => Log(LogCategory.Copy, message);
    public static void LogSuccess(string message) => Log(LogCategory.Success, message);
    public static void LogWatch(string message) => Log(LogCategory.Watch, message);
    public static void LogWarning(string message) => Log(LogCategory.Warning, message);
    public static void LogError(string message) => Log(LogCategory.Error, message);

    private static void Log(LogCategory category, string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"[{timestamp}] ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        if (category == LogCategory.Success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        else if (category == LogCategory.Warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else if (category == LogCategory.Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        Console.Write($"{category.ToString().ToLower()}");
        Console.ResetColor();
        Console.Write(" ");
        Console.WriteLine(message);
    }
}

public enum LogCategory
{
    Build,
    Info,
    Copy,
    Success,
    Watch,
    Warning,
    Error
}
