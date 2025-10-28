using Lesson.DTOs;
using Lesson.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lesson.Controllers
{
    // Lesson : Routing - Attribute Based
    // [ApiController]  => API Controller'ı olduğunu belirtir, otomatik Model Validation ve 400 Bad Request gibi özellikleri etkinleştirir.

    [ApiController]
    // [Route("api/[controller]")]: Bu Controller'a nasıl erişileceğini belirler.
    // [controller] ifadesi, sınıf adının 'Controller' kısmı atılarak kullanılır.
    // Yani bu 'ProductsController' için rota: /api/products
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        // Lesson: Dependency Injection (Bağımlılık Enjeksiyonu)
        // Controller, IProductService'i constructor üzerinden talep ediyor.
        // DI container (Program.cs) bu bağımlılığı otomatik olarak sağlar.

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // Lesson: Action Methods (Eylem Metotları)
        // [HttpGet("{id}")] => Bu metot, HTTP GET isteği ile /api/products/{id} yoluna yanıt verir. (Routing)

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();

            // Lesson: Status Code (Durum Kodu) / REST Principles
            // 'Ok()' metodu, HTTP 200 (OK) durum kodu ile birlikte 'products' listesini JSON formatında döner.
            return Ok(products);
        }

        // GET /api/products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                // Lesson: Status Code (Durum Kodu) / REST Principles
                // Kaynak bulunamadığında HTTP 404 (Not Found) dönmek, REST prensiplerinin gereğidir.
                return NotFound();
            }

            return Ok(product);
        }

        // POST /api/products
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto createDto)
        {
            // Lesson: Model Validation (Otomatik)
            // [ApiController] attribute'u sayesinde, eğer 'createDto'
            // DTO'daki kurallara (örn: [Required]) uymazsa, bu metot hiç çalışmaz ve framework otomatik 400 Bad Request döner.

            var newProduct = await _productService.CreateProductAsync(createDto);

            // Lesson: Status Code (Durum Kodu) / REST Principles
            // Bir kaynak oluşturulduğunda, HTTP 201 (Created) dönülür.
            // 'CreatedAtAction', cevabın 'Location' header'ına yeni oluşturulan kaynağın URL'ini (örn: /api/products/6) ekler. Bu, REST'in en önemli kurallarından biridir (HATEOAS).
            return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
        }

        // Lesson: Idempotency (Tekrarlanabilirlik)
        // PUT ve DELETE operasyonları 'idempotent' (etkisi aynı) olmalıdır.
        // Yani, aynı PUT isteğini 1 kez de atsan, 100 kez de atsan,sunucudaki sonuç (kaynağın son durumu) aynı kalır.

        // PUT /api/products/5 (Güncelleme)
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                // Lesson: Status Code (400 Bad Request)
                // URL'deki Id ile gövdedeki (body) Id uyuşmuyorsa bu bir 'kötü istektir'.
                return BadRequest();
            }

            // (Güncelleme servisi çağrılır...)

            // Lesson: Status Code (204 No Content)
            // Güncelleme başarılı olduğunda ve geriye bir veri dönmeye (body) gerek olmadığında 204 dönülür.
            return NoContent();
        }

        // DELETE /api/products/5 (Silme)
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // (Silme servisi çağrılır...)

            // Başarılı silme sonrası da 204 dönülebilir.
            return NoContent();
        }
    }
}
