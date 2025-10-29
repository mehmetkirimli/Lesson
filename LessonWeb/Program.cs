using LessonWeb.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);


#region Servisleri DI k?sm?na Ekleme (Ba??ml?l?k Enjeksiyonu)

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Lesson: HttpClientFactory (?stemci Yap?land?rmas?)
// 1. API'mizle (Lesson) konu?mak için bir HttpClient servisi ekliyoruz.
builder.Services.AddHttpClient("ApiHttpClient", client =>
{
    // Lesson: Güvenlik (HTTPS Zorlamas? ve Adres)
    // API'mizin çal??t??? adresi (ve portu) buraya yazmal?y?z.
    // 'dotnet run' ile API'yi çal??t?rd???nda sana verdi?i adresi kullan.
    // ÖNEML?: API projenin (Lesson) 'Properties/launchSettings.json' dosyas?ndan 'https' adresini bulabilirsin.

    // Örnek bir adres, KEND? ADRES?NLE DE???T?R:
    client.BaseAddress = new Uri("https://localhost:7220");
});

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
