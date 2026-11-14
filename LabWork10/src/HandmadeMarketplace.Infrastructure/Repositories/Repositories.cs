using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Enums;
using HandmadeMarketplace.Domain.Filters;
using HandmadeMarketplace.Domain.Interfaces;
using HandmadeMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HandmadeMarketplace.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        DbSet.FirstOrDefaultAsync(u => u.Email == email.ToLower(), ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        DbSet.AnyAsync(u => u.Email == email.ToLower(), ct);

    public async Task<IReadOnlyList<User>> GetArtisansAsync(CancellationToken ct = default) =>
        await DbSet.AsNoTracking()
            .Where(u => u.Role == UserRole.Artisan)
            .OrderBy(u => u.ShopName)
            .ToListAsync(ct);
}

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public override async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default) =>
        DbSet.AnyAsync(c => c.Name.ToLower() == name.ToLower(), ct);
}

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public Task<Product?> GetWithDetailsAsync(int id, CancellationToken ct = default) =>
        DbSet.Include(p => p.Artisan)
             .Include(p => p.Category)
             .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();
        return await DbSet.Where(p => idList.Contains(p.Id)).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Product>> GetByArtisanAsync(int artisanId, CancellationToken ct = default) =>
        await DbSet.AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Artisan)
            .Where(p => p.ArtisanId == artisanId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> SearchAsync(
        ProductFilter filter, CancellationToken ct = default)
    {
        IQueryable<Product> query = DbSet.AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Artisan)
            .Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var pattern = $"%{filter.Search.Trim()}%";
            query = query.Where(p => EF.Functions.Like(p.Title, pattern)
                                  || EF.Functions.Like(p.Description, pattern)
                                  || (p.Materials != null && EF.Functions.Like(p.Materials, pattern)));
        }

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId);
        if (filter.ArtisanId.HasValue)
            query = query.Where(p => p.ArtisanId == filter.ArtisanId);
        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);
        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);
        if (filter.OnlyCustomizable == true)
            query = query.Where(p => p.IsCustomizable);
        if (filter.OnlyInStock)
            query = query.Where(p => p.Stock > 0);

        query = filter.SortBy?.ToLowerInvariant() switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "rating" => query.OrderByDescending(p => p.AverageRating).ThenByDescending(p => p.ReviewsCount),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }

    public Task<Order?> GetWithItemsAsync(int id, CancellationToken ct = default) =>
        DbSet.Include(o => o.Items).ThenInclude(i => i.Product)
             .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<Order>> GetByBuyerAsync(int buyerId, CancellationToken ct = default) =>
        await DbSet.AsNoTracking()
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Where(o => o.BuyerId == buyerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public Task<bool> HasDeliveredProductAsync(int buyerId, int productId, CancellationToken ct = default) =>
        DbSet.AnyAsync(o => o.BuyerId == buyerId
                         && o.Status == OrderStatus.Delivered
                         && o.Items.Any(i => i.ProductId == productId), ct);
}

public class CustomOrderRepository : Repository<CustomOrderRequest>, ICustomOrderRepository
{
    public CustomOrderRepository(AppDbContext context) : base(context) { }

    private IQueryable<CustomOrderRequest> WithDetails() =>
        DbSet.Include(c => c.Buyer)
             .Include(c => c.Artisan)
             .Include(c => c.BaseProduct);

    public Task<CustomOrderRequest?> GetWithDetailsAsync(int id, CancellationToken ct = default) =>
        WithDetails().FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<CustomOrderRequest>> GetByBuyerAsync(int buyerId, CancellationToken ct = default) =>
        await WithDetails().AsNoTracking()
            .Where(c => c.BuyerId == buyerId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CustomOrderRequest>> GetByArtisanAsync(int artisanId, CancellationToken ct = default) =>
        await WithDetails().AsNoTracking()
            .Where(c => c.ArtisanId == artisanId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);
}

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Review>> GetByProductAsync(int productId, CancellationToken ct = default) =>
        await DbSet.AsNoTracking()
            .Include(r => r.Buyer)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public Task<bool> ExistsForBuyerAsync(int buyerId, int productId, CancellationToken ct = default) =>
        DbSet.AnyAsync(r => r.BuyerId == buyerId && r.ProductId == productId, ct);
}
