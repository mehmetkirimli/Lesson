using System.ComponentModel.DataAnnotations; // Bu 'using' satırı en üstte olmalı

namespace LessonWeb.DTOs
{
    // Lesson: 'record' yerine 'class' kullanıyoruz
    // Sebebi: Blazor'un '@bind-Value' komutu, form verisini modele yazabilmek için 'set;' accessor'ına (erişimcisine) ihtiyaç duyar.
    // 'record' ise varsayılan olarak 'init;' kullanır ve bu da CS0852 hatasına yol açar.
    public class CreateProductDto
    {
        [Required(ErrorMessage = "İsim alanı zorunludur.")]
        public string Name { get; set; } // 'init;' DEĞİL 'set;'

        [Range(0.01, 10000.00)]
        public decimal Price { get; set; } // 'init;' DEĞİL 'set;'

        public string? Description { get; set; } // 'init;' DEĞİL 'set;'


        // Lesson: Parametresiz Constructor (Yapıcı Metot)
        // Blazor'un 'EditForm'u modelin boş bir örneğini
        // oluşturabilmek için buna ihtiyaç duyar.
        public CreateProductDto()
        {
            Name = ""; // Boş string ataması [Required] hatasını tetiklemez
        }

        // AddProduct.razor'da 'new CreateProductDto("", 0, null)' yazdığın satırın çalışmaya devam etmesi için bu constructor'ı da ekliyoruz.
        public CreateProductDto(string name, decimal price, string? description)
        {
            Name = name;
            Price = price;
            Description = description;
        }
    }
}