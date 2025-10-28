using System.Linq.Expressions;

namespace Lesson.Repositories
{
    // Lesson: Generic Repository Pattern
    // Repository Pattern, veri erişim katmanını soyutlamak için kullanılan bir tasarım desenidir.

    // Generic Repository, tek bir repository sınıfının birden çok entity türü için kullanılabilmesini sağlar.
    // Bu, kod tekrarını azaltır ve bakımını kolaylaştırır.

    // SOLID Prensipleri'nden 'D' (Dependency Inversion) prensibine uygundur.

    // SOLID, nesne yönelimli tasarımda beş temel prensibi ifade eder:
    // 1-Single Responsibility, 2-Open/Closed, 3-Liskov Substitution, 4-Interface Segregation, 5-Dependency Inversion.

    // 1-Single Responsibility Principle (SRP): Bir sınıfın sadece bir sorumluluğu olmalıdır.
    // 2-Open/Closed Principle (OCP): Sınıflar genişletilmeye açık, değişikliğe kapalı olmalıdır.
    // 3-Liskov Substitution Principle (LSP): Türetilmiş sınıflar, temel sınıfların yerine geçebilmelidir.
    // 4-Interface Segregation Principle (ISP): Büyük arayüzler yerine, spesifik arayüzler kullanılmalıdır.
    // 5-Dependency Inversion Principle (DIP): Yüksek seviyeli modüller, düşük seviyeli modüllere bağımlı olmamalıdır; her ikisi de soyutlamalara bağlı olmalıdır.
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> Query(); // gelişmiş sorgu için
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);

        Task<int> SaveChangesAsync(); // küçük projeler için yeterli
    }
}
