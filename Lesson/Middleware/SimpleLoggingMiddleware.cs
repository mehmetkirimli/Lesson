using System;
using System.Diagnostics;
using Lesson.Helper;
using static System.Net.WebRequestMethods;

namespace Lesson.Middleware
{
    // Lesson: Middleware (Ara Yazılım Katmanı)
    // Middleware, HTTP isteği API'ye ulaştıktan ve API cevap ürettikten sonra araya girerek işlem yapmamızı sağlayan kod parçalarıdır.
    // Bir 'pipeline' (boru hattı) oluştururlar.
    // Örn: Request -> Logging Middleware -> Authentication -> Authorization -> *** API *** -> Response

    //Bu middleware, her HTTP isteği tamamlandığında(veya hata olduğunda) dosyaya bir özet yazar: hangi method/path, status code, süre.
    public class SimpleLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IFileLogger _logger;

        public SimpleLoggingMiddleware(RequestDelegate next, IFileLogger logger)
        {
            _next = next; // _next: Pipeline'daki bir sonraki adımı temsil eder.
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var method = context.Request.Method;
                var path = context.Request.Path + context.Request.QueryString;
                var source = "SimpleLoggingMiddleware";
                var action = $"{method} {path}";

                // Pipeline'daki bir sonraki adıma (örn: Controller'a) geç
                await _next(context);

                sw.Stop();
                var statusCode = context.Response?.StatusCode ?? 0;
                var summary = $"HTTP Request Handled {method} {path} => Responded {statusCode}";
                await _logger.LogAsync(source, action, summary, duration: sw.Elapsed, statusCode: statusCode);

            }
            catch (Exception ex) 
            {
                sw.Stop();
                var method = context.Request.Method;
                var path = context.Request.Path + context.Request.QueryString;
                var action = $"{method} {path}";
                var summary = "Unhandled exception in request pipeline";
                var detail = ex.ToString();
                await _logger.LogAsync("Middleware", action, summary, detail: detail, duration: sw.Elapsed, statusCode: 500);
                throw;
            }
        }
    }
}
