namespace Lesson.Entities
{
    // lesson: Abstract Class (Soyut Sınıf)
    // 'abstract' sınıflar 'new' anahtar sözcüğü ile doğrudan oluşturulamazlar.
    // Sadece miras (inheritance) yoluyla kullanılabilirler.
    // Ortak özellikleri (Id gibi) tek bir yerde toplamak için idealdirler.
    // Doğrudan bir nesne yaratamadığımız için sadece miras alınmak için tasarlanmış birer şablon görevi görürler.

    // lesson: Public / Private / Protected (Erişim Belirleyiciler)
    // 'public': Her yerden erişilebilir.
    // 'protected': Sadece bu sınıftan miras alan sınıflar erişebilir.
    // 'private': Sadece bu sınıfın içinden erişilebilir.
    // 'set' metodunu 'protected' yaparak, Id'nin dışarıdan değiştirilmesini engelleriz
    //  ama miras alan sınıfın (Product) bu Id'yi set edebilmesine izin veririz.
    public abstract class BaseEntity
    {
        public int Id { get; protected set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
