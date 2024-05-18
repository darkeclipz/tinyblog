namespace TinyBlog;

public static class Logger
{
    private static IConsole _console = new SystemConsole();

    // ReSharper disable once UnusedMember.Global
    public static void SetConsole(IConsole console)
    {
        _console = console;
    }
    
    public static void LogBuild(string message) => Log(LogCategory.Build, message);
    public static void LogInfo(string message) => Log(LogCategory.Info, message);
    public static void LogCreate(string message) => Log(LogCategory.Create, message);
    public static void LogCopy(string message) => Log(LogCategory.Copy, message);
    public static void LogSuccess(string message) => Log(LogCategory.Success, message);
    public static void LogWatch(string message) => Log(LogCategory.Watch, message);
    public static void LogWarning(string message) => Log(LogCategory.Warning, message);
    public static void LogError(string message) => Log(LogCategory.Error, message);

    private static void Log(LogCategory category, string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        _console.SetForegroundColor(ConsoleColor.DarkGray);
        _console.Write($"[{timestamp}] ");
        _console.SetForegroundColor(category switch
        {
            LogCategory.Success => ConsoleColor.Green,
            LogCategory.Warning => ConsoleColor.Yellow,
            LogCategory.Error => ConsoleColor.Red,
            _ => ConsoleColor.Cyan
        });
        _console.Write($"{category.ToString().ToLower()}");
        _console.ResetColor();
        _console.Write(" ");
        _console.WriteLine(message);
    }
}

public interface IConsole
{
    void Write(string message);
    void WriteLine(string message);
    void ResetColor();
    void SetForegroundColor(ConsoleColor foregroundColor);
}

public class SystemConsole : IConsole
{
    public void Write(string message) => Console.Write(message);
    public void WriteLine(string message) => Console.WriteLine(message);
    public void ResetColor() => Console.ResetColor();
    public void SetForegroundColor(ConsoleColor foregroundColor) => Console.ForegroundColor = foregroundColor;
}

public enum LogCategory
{
    Build,
    Info,
    Create,
    Copy,
    Success,
    Watch,
    Warning,
    Error
}
