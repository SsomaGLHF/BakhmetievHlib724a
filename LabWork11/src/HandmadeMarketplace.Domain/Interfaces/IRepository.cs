using HandmadeMarketplace.Domain.Common;

namespace HandmadeMarketplace.Domain.Interfaces;

/// <summary>
/// Узагальнений репозиторій з базовими CRUD-операціями.
/// Зміни фіксуються через <see cref="IUnitOfWork"/>.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
}

/// <summary>
/// Одиниця роботи: зберігає всі зміни, зроблені репозиторіями, однією транзакцією.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
