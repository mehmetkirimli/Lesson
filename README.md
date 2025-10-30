# .NET Eğitim Projesi: Full-Stack .NET (API + Blazor + SignalR)

Bu proje, modern .NET ekosistemindeki temel ve ileri düzey kavramları uygulamalı olarak göstermek amacıyla oluşturulmuş bir "Full-Stack" (Tam Yığın) uygulamadır. Proje, teknik mülakatlarda karşılaşılabilecek senaryoları (performans, mimari, gerçek zamanlı iletişim) kapsamaktadır.

Proje iki ana bölümden oluşur:

1.  **`Lesson` (Backend):** PostgreSQL veritabanı üzerinde çalışan, Repository desenini kullanan, SignalR ile anlık bildirimler gönderen bir ASP.NET Core Web API.
2.  **`LessonWeb` (Frontend):** Bu API ile konuşan, `HttpClient` ile veri çeken ve `SignalR` Hub'ını dinleyerek arayüzünü anlık olarak güncelleyen bir Blazor Server uygulaması.

-----

## 🚀 Kapsanan Temel Eğitim Konuları

  * **Altyapı:** Docker Compose (PostgreSQL yönetimi)
  * **API:** ASP.NET Core Web API, REST Prensipleri
  * **Mimari:** Repository Deseni (Generic & Specific), Dependency Injection (DI)
  * **Veritabanı:** Entity Framework Core (Code-First), Migrations, LINQ, `AsNoTracking()`
  * **Gerçek Zamanlı İletişim:** SignalR (Sunucudan istemciye anlık veri aktarımı)
  * **Güvenlik / Yapılandırma:** CORS (Kaynaklar Arası Kaynak Paylaşımı)
  * **Frontend:** Blazor Server
  * **Blazor (İleri Seviye):** `HttpClientFactory`, `EditForm` ile veri gönderme (`POST`), `StateHasChanged()` ile UI güncelleme, `@bind-Value` (`set;` vs `init;` sorunu)

-----

## 🏗️ Mimari ve Proje Yapısı

### 1\. `Lesson` (Backend API)

Bu proje, veritabanı işlemlerini, iş mantığını ve dış dünyaya açılan kapıları (API) yönetir.

  * **`docker-compose.yml`:** Geliştirme ortamı için gerekli `PostgreSQL` veritabanını tek komutla (`docker-compose up`) ayağa kaldırır. Verilerin kalıcı olması (persistence) için `volumes` kullanılmıştır.
  * **`Data/AppDbContext.cs`:** Entity Framework Core'un veritabanı ile konuştuğu ana sınıftır.
  * **`Repositories/`:** Veritabanı sorgu mantığını, iş katmanından (Servisler) soyutlar.
      * `IRepository.cs`: Temel CRUD işlemleri için Jenerik (Generic) arayüz.
      * `IProductRepository.cs`: `Product`'a özel sorgular için arayüz.
  * **`Services/`:** İş mantığının (business logic) bulunduğu katmandır. `IProductRepository`'ye bağımlıdır (Dependency Inversion).
  * **`Controllers/`:** Dış dünyadan gelen HTTP isteklerini karşılayan ve `IProductService`'i çağıran katmandır.
  * **`DTOs/`:** Katmanlar arası (ve API-İstemci arası) veri taşımak için kullanılan modellerdir (`ProductDto`, `CreateProductDto`).
  * **`Hubs/ProductHub.cs`:** SignalR iletişim merkezidir. `ProductService`, yeni bir ürün eklendiğinde bu Hub'ı tetikler.
  * **`Program.cs`:**
      * **Dependency Injection (DI):** Tüm servislerin (`AddScoped<IProductService, ProductService>`) yaşam döngülerinin yönetildiği yerdir.
      * **Middleware:** `SimpleLoggingMiddleware` gibi özel ara yazılımların ve `UseCors` gibi yerleşik ara yazılımların yapılandırıldığı yerdir.
      * **CORS:** `LessonWeb` (`https:localhost:XXXX`) gibi farklı bir adresten gelen `SignalR` ve `HTTP` isteklerine izin vermek için `WithOrigins(...)` ve `AllowCredentials()` ile yapılandırılmıştır.

### 2\. `LessonWeb` (Frontend Blazor)

Bu proje, son kullanıcının gördüğü arayüzdür. API'den aldığı veriyi gösterir ve API'ye veri gönderir.

  * **`Program.cs`:**
      * **`HttpClientFactory`:** API (`Lesson`) projesiyle konuşmak için bir `HttpClient`'ı DI konteynerine kaydeder ve `BaseAddress`'i (`https://api-adresi.com`) burada yapılandırılır.
      * **`SignalR.Client` Paketi:** API'deki Hub'a bağlanmak için gereken istemci kütüphanesi (NuGet) projeye eklenmiştir.
  * **`DTOs/`:** API ile aynı veri "kontratını" (şeklini) konuşabilmek için API'deki DTO'lar buraya manuel olarak (`record` yerine `class` olarak) kopyalanmıştır.
  * **`Pages/FetchData.razor`:**
      * `OnInitializedAsync`: Sayfa yüklenirken `HttpClient` kullanarak API'den (`GET /api/products`) ilk veriyi çeker.
      * `HubConnection`: API'deki `/productHub` adresine anlık bir bağlantı kurar.
      * `hubConnection.On<ProductDto>("ProductCreated", ...)`: Sunucudan "ProductCreated" mesajı geldiğinde, listeye yeni ürünü ekler ve `InvokeAsync(StateHasChanged)` çağrısı ile arayüzü anında günceller.
      * `IAsyncDisposable`: Sayfadan çıkıldığında `hubConnection`'ı sonlandırarak bellek sızıntılarını (memory leak) engeller.
  * **`Pages/AddProduct.razor`:**
      * **`EditForm` & `DataAnnotationsValidator`:** C\# DTO'sundaki (`[Required]` gibi) kuralları kullanarak istemci tarafında form doğrulaması yapar.
      * **`@bind-Value`:** Form elemanlarını `CreateProductDto` modeline bağlar. (Modelin `init;` değil, `set;` kullanması gerektiği burada öğrenilmiştir.)
      * **`OnValidSubmit`:** Form geçerli olduğunda, `HttpClient.PostAsJsonAsync` kullanarak API'ye (`POST /api/products`) yeni ürünü gönderir.

-----

## 🛠️ Kurulum ve Çalıştırma

Bu projeyi yerel makinenizde çalıştırmak için iki projenin de ayarlanması gerekir.

**Ön Gereksinimler:**

  * .NET 8.0 SDK (veya 6.0/7.0 SDK'ları)
  * Docker Desktop

### 1\. Backend (`Lesson` API) Kurulumu

1.  **Docker'ı Başlatın:** Projenin ana dizininde bir terminal açın ve PostgreSQL veritabanını başlatın:
    ```sh
    docker-compose up -d
    ```
2.  **Veritabanını Oluşturun:** `Lesson` projesinin `appsettings.json` dosyasındaki `ConnectionString`'in, `docker-compose.yml` içindeki `POSTGRES_USER/PASSWORD` ile eşleştiğinden emin olun.
    Ardından Package Manager Console (PMC) üzerinden migration'ları çalıştırın:
    ```powershell
    # PM> (Default Project: Lesson)
    Update-Database
    ```

### 2\. Frontend (`LessonWeb`) Kurulumu

1.  **API Adresini Tanımlayın:** `LessonWeb` projesindeki `Program.cs` dosyasını açın. `builder.Services.AddHttpClient` bloğundaki `BaseAddress`'i, `Lesson` (API) projenizin `launchSettings.json` dosyasında bulunan `https` adresiyle (örn: `https://localhost:7207`) güncelleyin.
2.  **CORS Adresini Tanımlayın:** `Lesson` (API) projesindeki `Program.cs` dosyasını açın. `policy.WithOrigins(...)` içine, `LessonWeb` projesinin `launchSettings.json` dosyasında bulunan `https` adresini (örn: `https://localhost:7157`) yazın.

### 3\. Projeyi Çalıştırma

1.  Visual Studio'da en üstteki `Solution 'Lesson'`'a sağ tıklayın.
2.  `Properties` -\> `Startup Project` -\> `Multiple startup projects`'i seçin.
3.  `Lesson` ve `LessonWeb` projelerinin ikisi için de `Action` sütununu `Start` olarak ayarlayın.
4.  `Apply` (Uygula) ve `OK`'a basın.
5.  Visual Studio'da `F5`'e (veya yeşil Oynat tuşuna) basın.

İki proje de başlayacak, iki ayrı tarayıcı sekmesi açılacaktır (Biri Swagger, biri Blazor arayüzü).

-----

## 🧪 Test Senaryosu

1.  Blazor arayüzünde (`LessonWeb`) "Fetch data" sayfasına gidin. (Liste boş veya dolu gelecektir).
2.  Swagger arayüzünde (`Lesson`) `POST /api/products`'u kullanarak yeni bir ürün ekleyin.
3.  **Anında** Blazor sekmesine geri dönün.
4.  Sayfayı yenilemenize gerek kalmadan, yeni eklediğiniz ürünün listenin sonuna **gerçek zamanlı (real-time)** olarak eklendiğini gözlemleyin.
