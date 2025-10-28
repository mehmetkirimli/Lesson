using System.Text;

namespace Lesson.Helper
{
    public class FileLogger : IFileLogger
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public FileLogger(IWebHostEnvironment env)
        {
            // Proje dizini (content root) altına dosya
            _filePath = Path.Combine(env.ContentRootPath, "LessonLogFile.txt");
        }

        public async Task LogAsync(string source, string action, string summary, string? detail = null, TimeSpan? duration = null, int? statusCode = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine(new string('-', 70));
            sb.AppendLine($"Tarih : {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} (UTC)");
            sb.AppendLine($"Kaynak : {source}");
            sb.AppendLine($"İşlem  : {action}");
            if (statusCode.HasValue) sb.AppendLine($"Status : {statusCode}");
            if (duration.HasValue) sb.AppendLine($"Süre   : {duration?.TotalMilliseconds:F0} ms");
            sb.AppendLine($"Özet   : {summary}"); // kullanıcı istediği 'özet' burada
            if (!string.IsNullOrWhiteSpace(detail))
            {
                sb.AppendLine("Detay  :");
                sb.AppendLine(detail);
            }
            sb.AppendLine(); // boş satır

            var text = sb.ToString();

            await _semaphore.WaitAsync();
            try
            {
                // Dosyaya ekle (append)
                await File.AppendAllTextAsync(_filePath, text);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
