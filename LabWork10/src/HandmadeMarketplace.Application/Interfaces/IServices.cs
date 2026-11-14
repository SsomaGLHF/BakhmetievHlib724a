using HandmadeMarketplace.Application.Common;
using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Domain.Enums;

namespace HandmadeMarketplace.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<UserDto>> GetArtisansAsync(CancellationToken ct = default);
    Task<UserDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<UserDto> RegisterAsync(CreateUserDto dto, CancellationToken ct = default);
}

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<CategoryDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default);
}

public interface IProductService
{
    Task<PagedResult<ProductDto>> SearchAsync(ProductQueryDto query, CancellationToken ct = default);
    Task<ProductDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<ProductDto>> GetByArtisanAsync(int artisanId, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default);
    Task DeactivateAsync(int id, int artisanId, CancellationToken ct = default);
}

public interface IOrderService
{
    Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default);
    Task<OrderDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<OrderDto>> GetByBuyerAsync(int buyerId, CancellationToken ct = default);
    Task<OrderDto> ChangeStatusAsync(int id, OrderStatus newStatus, CancellationToken ct = default);
}

public interface ICustomOrderService
{
    Task<CustomOrderDto> CreateAsync(CreateCustomOrderDto dto, CancellationToken ct = default);
    Task<CustomOrderDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<CustomOrderDto>> GetForBuyerAsync(int buyerId, CancellationToken ct = default);
    Task<IReadOnlyList<CustomOrderDto>> GetForArtisanAsync(int artisanId, CancellationToken ct = default);

    // Дії майстра
    Task<CustomOrderDto> QuoteAsync(int id, QuoteCustomOrderDto dto, CancellationToken ct = default);
    Task<CustomOrderDto> RejectAsync(int id, int artisanId, string? comment, CancellationToken ct = default);
    Task<CustomOrderDto> StartAsync(int id, int artisanId, CancellationToken ct = default);
    Task<CustomOrderDto> CompleteAsync(int id, int artisanId, CancellationToken ct = default);

    // Дії покупця
    Task<CustomOrderDto> AcceptAsync(int id, int buyerId, CancellationToken ct = default);
    Task<CustomOrderDto> CancelAsync(int id, int buyerId, CancellationToken ct = default);
}

public interface IReviewService
{
    Task<IReadOnlyList<ReviewDto>> GetForProductAsync(int productId, CancellationToken ct = default);
    Task<ReviewDto> CreateAsync(int productId, CreateReviewDto dto, CancellationToken ct = default);
}
