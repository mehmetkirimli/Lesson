using Lesson.Data;
using Lesson.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

// Lesson: Generic Repository Pattern
// SaveChangesAsync metodu eklenerek, birim işlemler (unit of work) desteklenmiştir.

// Repository Pattern, veri erişim katmanını soyutlamak için kullanılan bir tasarım desenidir.
// Transaction yönetimi için DbContext'in SaveChangesAsync metodu kullanılır.

// AsTracking kullanımı, performansı artırır ve bellek kullanımını azaltır. Kısaca => Veriyi manipüle edecekseniz AsTracking kullanın.
// Eğer entity üzerinde değişiklik yapılmayacaksa AsNoTracking tercih edilmelidir. Kısaca => Sadece okuma işlemi için AsNoTracking kullanın.
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        // Basit: primary key ile arama (composite key varsa ayrı işlem)
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public IQueryable<T> Query()
    {
        // Kontrollü bir şekilde IQueryable veriyoruz; servis/consumer dikkat etsin.
        return _dbSet.AsQueryable();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }


    // Lesson: Unit of Work Deseni
    // Değişikliklerin toplu olarak veritabanına uygulanmasını sağlar.
    // Transaction'ları yönetmek için de bu katman idealdir.
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
