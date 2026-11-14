using HandmadeMarketplace.Domain.Common;
using HandmadeMarketplace.Domain.Interfaces;
using HandmadeMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HandmadeMarketplace.Infrastructure.Repositories;

/// <summary>
/// Базова реалізація репозиторію на Entity Framework Core.
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await DbSet.FindAsync(new object[] { id }, ct);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.AsNoTracking().ToListAsync(ct);

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
        DbSet.AnyAsync(e => e.Id == id, ct);

    public async Task AddAsync(T entity, CancellationToken ct = default) =>
        await DbSet.AddAsync(entity, ct);

    public void Update(T entity) => DbSet.Update(entity);

    public void Remove(T entity) => DbSet.Remove(entity);
}

/// <summary>
/// Unit of Work: усі репозиторії в межах HTTP-запиту ділять один scoped AppDbContext,
/// тому один виклик SaveChanges фіксує всі зміни атомарно.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context) => _context = context;

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
}
