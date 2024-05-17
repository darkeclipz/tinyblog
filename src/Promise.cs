namespace TinyBlog;

public class Promise
{
    private bool IsSuccess { get; init; }
    public static readonly Promise Success = Create(true);
    public static readonly Promise Failed = Create(false);

    private static Promise Create(bool success)
    {
        return new Promise()
        {
            IsSuccess = success
        };
    }

    public void OnSuccess(Action action)
    {
        if (IsSuccess)
        {
            action();
        }
    }
}
