namespace Lesson.Helper
{
    public interface IFileLogger
    {
        Task LogAsync(string source, string action, string summary, string? detail = null, TimeSpan? duration = null, int? statusCode = null);
    }
}
