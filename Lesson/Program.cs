
using Lesson.Data;
using Lesson.Helper;
using Lesson.Hubs;
using Lesson.Middleware;
using Lesson.Repositories;
using Lesson.Services;
using Microsoft.EntityFrameworkCore;

namespace Lesson
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            /*
             * Lesson : Dependency Injection (DI) - (Lifetime - Yaşam Döngüleri)
             * 
             * 'builder.Services' koleksiyonu, .NET'in DI Container'ıdır. ***** Önemli !
             *  
             *  Buraya hangi 'interface' istendiğinde hangi 'class'ın örneğinin verileceğini kaydederiz.
             * 
             * AddScoped: (En yaygını) Nesne, her HTTP 'isteği' (request) için bir kez oluşturulur ve o istek boyunca kullanılır. (örn: AppDbContext, ProductService)
             * AddTransient: Nesne, her 'talep edildiğinde' (enjekte edildiğinde) yeni bir örnek olarak oluşturulur.
             * AddSingleton: Nesne, uygulama başladığında bir kez oluşturulur ve uygulama kapanana kadar hep aynı örnek kullanılır. (örn: ILogger, Caching)
             * 
             * 
             */

            #region 1- Builder.Services DI Container'a Servis Ekleme

            builder.Services.AddControllers();

            builder.Services.AddSignalR(); // SignalR servisi
            builder.Services.AddEndpointsApiExplorer(); // Learn about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddSwaggerGen();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddSingleton<IFileLogger, FileLogger>();

            #endregion

            // HTTP Request Pipeline (Middleware Sırası Önemlidir!) ---
            // Lesson: Middleware (Pipeline)
            // İstekler bu 'Use...' metotlarından sırayla geçer.

            var app = builder.Build();

            app.Lifetime.ApplicationStarted.Register(() => { Console.WriteLine($"App started (ApplicationStarted) in {app.Environment.EnvironmentName}"); }); // Hangi mod açık ise onu yazar (Development, Production, Staging)    

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseMiddleware<SimpleLoggingMiddleware>(); // Bizim özel middleware'ı ekliyoruz.
            //Authentication varsa burada eklenir , öncesinde de builder.Services.AddAuthentication(...) eklenir.
            app.UseAuthorization();

            // İstemcilerin (Blazor, JavaScript) bağlanacağı adres budur.
            app.MapHub<ProductHub>("/productHub");

            // Lesson: Routing (Yönlendirme)
            // 'UseRouting' ve 'MapControllers' birlikte çalışır. UseRouting: Gelen URL'i analiz eder ve hangi 'Endpoint'in (Controller Action) çalışacağına karar verir.
            app.MapControllers();

            app.Run();
        }
    }
}
