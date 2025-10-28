using Lesson.Data;
using Lesson.DTOs;
using Lesson.Entities;
using Lesson.Repositories;
using Microsoft.AspNetCore.Hosting.Server;

namespace Lesson.Services
{
    public class ProductService : IProductService   
    {
        // Lesson: Readonly (Sadece Okunabilir)
        // 'readonly' olarak işaretlenen bir değişken, sadece 2 yerde atanabilir.
        // 1.Tanımlandığı satırda --- 2.'constructor' (yapıcı metot) içinde.
        // Sonradan değiştirilemez. Bu, 'dependency'lerin yanlışlıkla değiştirilmesini engeller.

        //private readonly AppDbContext _context;
        private readonly IRepository<Product> repo;
        private readonly AppDbContext _context;

        // Lesson: Dependency Injection (DI) - 2. Kural (Constructor Injection)
        // Bu sınıf, ihtiyaç duyduğu 'AppDbContext'i 'new' ile kendi oluşturmaz.
        // DI container'dan (Program.cs) constructor aracılığıyla ister.

        public ProductService(IRepository<Product> productRepo, AppDbContext context)
        {
            repo = productRepo;
            _context = context;
        }

        // Lesson: DB çağrıları I/O bound; async/await kullanmak uygulamanın thread havuzunu verimli kullanmasını sağlar(özellikle web server).

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            // Lesson: AsNoTracking (İzlemeyi Kapatma)
            // PERFORMANS: Sadece veri okuyacaksak ve üzerinde değişiklik yapmayacaksak,'AsNoTracking()' kullanmak EF Core'un bu veriyi 'izlemesini' (track) engeller.
            // Bu 'AsNoTracking' yapısını repository katmanında düşündük bu sebeple.
            // Bu, bellek kullanımını azaltır ve sorguyu hızlandırır.
            var product = await repo.GetByIdAsync(id);

            if (product == null) return null;

            // Manuel olarak Entity -> DTO dönüşümü (AutoMapper gibi araçlar da kullanılır)
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description
            };
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            // Lesson: LINQ (Language Integrated Query)
            // SQL yazar gibi C# kodu yazmamızı sağlar.
            // Bu kod, EF Core tarafından veritabanında çalışacak SQL'e çevrilir.

            // Lesson: LINQ (Where)
            // 'WHERE p.Price > 10' SQL sorgusuna karşılık gelir. Sadece fiyatı 10'dan büyük olanları getirir.
            var productsList = await repo.GetAllAsync();

            var filterList = productsList.Where(p=> p.Price >  0);

            // Lesson : IQueryable vs IEnumerable 
            // Iqueryable, sorgunun veritabanında oluşturulmasını sağlar. Sorgu üzerinde ek filtreler eklenebilir.
            // IEnumerable ise sorgunun veritabanında çalıştırılıp sonuçların belleğe alınmasını sağlar.

            // Lesson: LINQ (Select)
            // 'SELECT Id, Name, Price' SQL sorgusuna karşılık gelir.
            // Veritabanından sadece DTO için gerekli alanları çekeriz.
            // PERFORMANS: Tüm 'Product' kolonlarını çekmek yerine (örn: RowVersion)
            // sadece DTO'da gerekenleri çekmek (projection) çok daha performanslıdır.

            var responseList = filterList.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description
            });

            return responseList;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
        {
            var product = new Product
            {
                Name = createDto.Name,
                Price = createDto.Price,
                Description = createDto.Description
            };

            var data = repo.FindAsync(p => p.Name == createDto.Name);

            if(data != null)
            {
                throw new Exception("Bu isimde zaten bir ürün var.");
            }

            await repo.AddAsync(product);
            await repo.SaveChangesAsync(); // Değişiklikleri kaydet


            // Oluşturulan ürünü DTO olarak geri dönüyoruz.
            return (await GetProductByIdAsync(product.Id))!;
        }

        public async Task UpdateProductStockWithTransactionAsync(int id, int amount)
        {
            // Lesson: Transaction (İşlem Bütünlüğü)
            // Birbirine bağlı birden fazla veritabanı işlemini (örn: stok güncelle ve sipariş kaydı oluştur) tek bir 'iş parçası' olarak sarmalarız.
            // 'using' bloğu, işlem başarılı olursa 'Commit' (onaylar), hata olursa 'Rollback' (geri alır) işlemlerini yönetmemizi sağlar.
            // Ya hep ya hiç: İki işlem de başarılı olur veya ikisi de geri alınır.

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var product = await repo.GetByIdAsync(id);

                    if (product == null)
                    {
                        throw new Exception("Ürün bulunamadı");
                    }

                    // 1. İşlem: Fiyat Güncelleme (Stok olmadığını varsayalım)
                    product.Price = amount;
                    await _context.SaveChangesAsync(); // SaveChangesAsync (Tracking sayesinde 'Modified' olduğunu bilir)

                    // 2. İşlem: Başka bir tabloya log kaydı (varsayalım)
                    // _context.PriceLogs.Add(new PriceLog { ... });
                    // await _context.SaveChangesAsync();

                    // Eğer buraya kadar hata almadıysak, tüm işlemleri onayla.
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // Herhangi bir adımda hata olursa, tüm değişiklikleri geri al.
                    await transaction.RollbackAsync();

                    // Lesson: Garbage Collector (GC)
                    // 'ex' nesnesi, 'catch' bloğu bittiğinde 'scope' (kapsam) dışına çıkar ve 'yetim' kalır.
                    // .NET'in GC'si (Çöp Toplayıcı), böyle sahipsiz kalan nesneleri bellekten periyodik olarak otomatik temizler. Manuel (manuel) bellek yönetimine (C++'taki gibi) gerek kalmaz.

                    Console.WriteLine($"İşlem geri alındı: {ex.Message}");
                }
            }
        }

    }
}
