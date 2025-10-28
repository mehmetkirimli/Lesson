
using System.ComponentModel.DataAnnotations;

namespace Lesson.DTOs
{
    // Lesson: DTO (Data Transfer Object)
    // DTO'lar, katmanlar (örn: API Controller -> Servis) veya sistemler 
    // (örn: API -> Web Tarayıcı) arasında veri taşımak için kullanılan basit sınıflardır.
    // Asla veritabanı 'Entity'lerini (Product.cs) doğrudan API'ye açığa vurmayız.
    // Neden? 
    // 1. Güvenlik: (RowVersion, CreatedDate gibi hassas verileri gizleriz)
    // 2. Esneklik: (Entity değişse bile DTO sabit kalabilir)
    // 3. Validation: (Sadece o işleme özel doğrulama ekleyebiliriz)

    // Lesson: Record (Kayıt)
    // 'record'lar, C# 9+ ile gelen özel bir 'class' türüdür.
    // Özellikle 'değişmez' (immutable) veri taşımak için tasarlanmıştır.
    // Property'leri 'init' (sadece oluşturulurken değer atanabilir) olarak tanımlar.
    // DTO'lar için mükemmeldirler.
    public record CreateProductDto
    (
        // Lesson: Model Validation (DTO Seviyesi)
        // API'ye gelen JSON verisi bu kurallara uymuyorsa,
        // ASP.NET Core otomatik olarak '400 Bad Request' döner.

        [Required(ErrorMessage = "İsim alanı zorunludur.")]
        [StringLength(100)]
        string Name,


        [Range(0.01, 10000.00)]
        decimal Price,


        string? Description
    );
}
