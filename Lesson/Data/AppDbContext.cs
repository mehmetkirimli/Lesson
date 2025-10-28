using Lesson.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lesson.Data
{
    // Lesson: EF Core DbContext
    // DbContext, veri erişim köprüsü , veritabanı ile uygulama arasındaki ana bağlantı noktasıdır.
    // Veritabanı sorgularını (LINQ) SQL'e çevirir ve işlemleri yönetir.


    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        DbSet<Product> Products { get; set; } // DB'deki 'Products' tablosunu temsil eder.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Lesson: Migrations (Detaylı Yapılandırma)
            // Model (sınıf) üzerinde attribute ile belirtemediğimiz detaylı veritabanı yapılandırmaları burada yapılır.

            // Lesson: Index (İndeks)
            // 'Name' sütunu üzerinde bir indeks oluşturuyoruz.
            // Bu, 'WHERE Name = ...' gibi sorguların çok daha hızlı çalışmasını sağlar.
            // PERFORMANS artışı için kritik öneme sahiptir.

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name)
                .IsUnique(); // İsimlerin benzersiz (unique) olmasını da sağlayalım.

            // Lesson: Sequence (Sıra)
            // 'Identity' (otomatik artan Id) yerine, veritabanında merkezi bir
            // sayı üreteci (sequence) tanımlayabiliriz.
            // Özellikle Identity'nin yetersiz kaldığı karmaşık senaryolarda kullanılır.

            modelBuilder.HasSequence<int>("OrderNumbers")
                .StartsAt(1000)
                .IncrementsBy(1);
        }
    }
}
