using Lesson.DTOs;

namespace Lesson.Services
{
    // Lesson: Interface (Arayüz)
    // Interface, bir sınıfın hangi metotlara sahip olması gerektiğini söyleyen bir 'sözleşmedir'. Hiçbir metot gövdesi (kod) içermez.
    // 'I' ile başlaması bir kuraldır (IProductService, ILogger).

    // Lesson: Dependency Injection (DI) - 1. Kural (Arayüze Bağımlılık)
    // Sınıflar, diğer sınıflara doğrudan bağımlı olmamalıdır (ProductService'e değil).
    // Sınıflar, 'interface'lere bağımlı olmalıdır (IProductService'e).
    // Bu, 'ProductService' yerine 'MockProductService' (test için) kullanmamızı sağlar. Buna 'Dependency Inversion' denir.

    // Object Oriented Programming (Nesne Yönelimli Programlama) prensiplerinden biri olan Interface kullanımıdır.

    // Interface'ler 'new' anahtar sözcüğü ile doğrudan oluşturulamazlar.

    // Interface'ler, bir sınıfın hangi metotlara sahip olması gerektiğini belirtir, ancak bu metotların nasıl çalışacağını belirtmez.

    // Interface'ler, bir sınıfın birden fazla interface'i uygulamasına (implement) izin verir. Çoklu kalıtım (multiple inheritance) sağlarlar.

    // Object Oriented Programming'de 4 temel prensip vardır: Encapsulation, Abstraction, Inheritance, Polymorphism.

    // Interface'ler, Polymorphism (Çok Biçimlilik) sağlar. Farklı sınıflar aynı interface'i uygulayabilir ve farklı davranışlar sergileyebilir.

    // 
    public interface IProductService
    {
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> CreateProductAsync(CreateProductDto createDto);
        Task UpdateProductStockWithTransactionAsync(int id, int amount);
    }
}
