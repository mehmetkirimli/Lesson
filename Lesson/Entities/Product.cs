using System.ComponentModel.DataAnnotations;

namespace Lesson.Entities
{
    // BaseEntity sınıfından miras alır (Inheritance)
    // BaseEntity'deki ortak özelliklere sahiptir (Id, CreatedAt, IsDeleted)

    // Object Oriented Programming (Nesne Yönelimli Programlama) prensiplerinden biri olan Inheritance (Miras Alma) kullanılır.

    // Lesson: Model Validation (Veri Doğrulama) 'attribute'lar hem EF Core tarafından veritabanı kısıtlamaları oluşturur . (örn: NOT NULL) Bir de DTO'larda göreceğimiz gibi API'ye gelen veriyi doğrulamak için kullanılır.
    public class Product : BaseEntity
    {
        [Required]          // Bu attribute, Name property'sinin boş geçilmemesini sağlar.
        [StringLength(100)] // Bu attribute, Name property'sinin maksimum uzunluğunu 100 karakter ile sınırlar.
        public string Name { get; set; } = string.Empty;


        // Lesson: Nullable Reference Types
        // Projede 'Nullable' açık olduğu için, string'in null olabilmesini istiyorsak '?' kullanmak zorundayız. Aksi halde derleyici uyarır.
        public string? Description { get; set; }

        public decimal Price { get; set; }  


        // Lesson: Concurrency (Eşzamanlılık Yönetimi) - Optimistic Concurrency
        // [Timestamp] attribute'u, bu alanın veritabanı tarafından otomatik yönetilen bir 'rowversion' veya 'xmin' (PostgreSQL) sütunu olduğunu belirtir.
        // Bir kayıt güncellendiğinde, EF Core bu alanın DB'deki değeri ile elindeki değeri karşılaştırır. Farklıysa, 'DbUpdateConcurrencyException' fırlatır.
        // Bu, iki kullanıcının aynı anda aynı kaydı güncellemesini engeller.
        [Timestamp]
        public uint RowVersion { get; set; }
    }
}
