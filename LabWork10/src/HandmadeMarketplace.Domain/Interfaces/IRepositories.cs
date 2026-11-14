using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Filters;

namespace HandmadeMarketplace.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetArtisansAsync(CancellationToken ct = default);
}

public interface ICategoryRepository : IRepository<Category>
{
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
}

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetWithDetailsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetByArtisanAsync(int artisanId, CancellationToken ct = default);
    Task<(IReadOnlyList<Product> Items, int TotalCount)> SearchAsync(ProductFilter filter, CancellationToken ct = default);
}

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetWithItemsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByBuyerAsync(int buyerId, CancellationToken ct = default);

    /// <summary>Чи має покупець доставлене замовлення з цим товаром (для права на відгук).</summary>
    Task<bool> HasDeliveredProductAsync(int buyerId, int productId, CancellationToken ct = default);
}

public interface ICustomOrderRepository : IRepository<CustomOrderRequest>
{
    Task<CustomOrderRequest?> GetWithDetailsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<CustomOrderRequest>> GetByBuyerAsync(int buyerId, CancellationToken ct = default);
    Task<IReadOnlyList<CustomOrderRequest>> GetByArtisanAsync(int artisanId, CancellationToken ct = default);
}

public interface IReviewRepository : IRepository<Review>
{
    Task<IReadOnlyList<Review>> GetByProductAsync(int productId, CancellationToken ct = default);
    Task<bool> ExistsForBuyerAsync(int buyerId, int productId, CancellationToken ct = default);
}
