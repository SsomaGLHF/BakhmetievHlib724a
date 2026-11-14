using System.ComponentModel.DataAnnotations;
using HandmadeMarketplace.Domain.Enums;

namespace HandmadeMarketplace.Application.DTOs;

// ---------- Користувачі ----------

public record UserDto(int Id, string FullName, string Email, string Role, string? ShopName, string? Bio);

public class CreateUserDto
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Buyer;

    [StringLength(150)]
    public string? ShopName { get; set; }

    [StringLength(2000)]
    public string? Bio { get; set; }
}

// ---------- Категорії ----------

public record CategoryDto(int Id, string Name, string? Description);

public class CreateCategoryDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}

// ---------- Вироби ----------

public record ProductDto(
    int Id,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    string? Materials,
    bool IsCustomizable,
    int? ProductionDays,
    bool IsActive,
    double AverageRating,
    int ReviewsCount,
    int CategoryId,
    string CategoryName,
    int ArtisanId,
    string ArtisanShopName);

public class CreateProductDto
{
    /// <summary>Ідентифікатор майстра. У реальній системі береться з JWT-токена.</summary>
    [Range(1, int.MaxValue)]
    public int ArtisanId { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Required, StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 1_000_000)]
    public decimal Price { get; set; }

    [Range(0, 10_000)]
    public int Stock { get; set; }

    [StringLength(300)]
    public string? Materials { get; set; }

    public bool IsCustomizable { get; set; }

    [Range(1, 365)]
    public int? ProductionDays { get; set; }
}

public class UpdateProductDto
{
    [Range(1, int.MaxValue)]
    public int ArtisanId { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Required, StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 1_000_000)]
    public decimal Price { get; set; }

    [Range(0, 10_000)]
    public int Stock { get; set; }

    [StringLength(300)]
    public string? Materials { get; set; }

    public bool IsCustomizable { get; set; }

    [Range(1, 365)]
    public int? ProductionDays { get; set; }

    public bool IsActive { get; set; } = true;
}

public class ProductQueryDto
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public int? ArtisanId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? OnlyCustomizable { get; set; }
    public bool OnlyInStock { get; set; }
    public string? SortBy { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}

// ---------- Замовлення ----------

public record OrderItemDto(int ProductId, string ProductTitle, int Quantity, decimal UnitPrice, decimal LineTotal);

public record OrderDto(
    int Id,
    int BuyerId,
    string ShippingAddress,
    string Status,
    decimal Subtotal,
    decimal PlatformFee,
    decimal Total,
    DateTime CreatedAt,
    IReadOnlyList<OrderItemDto> Items);

public class CreateOrderItemDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; } = 1;
}

public class CreateOrderDto
{
    [Range(1, int.MaxValue)]
    public int BuyerId { get; set; }

    [Required, StringLength(500, MinimumLength = 10)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class UpdateOrderStatusDto
{
    [Required]
    public OrderStatus Status { get; set; }
}

// ---------- Кастомні замовлення ----------

public record CustomOrderDto(
    int Id,
    int BuyerId,
    string BuyerName,
    int ArtisanId,
    string ArtisanShopName,
    int? BaseProductId,
    string? BaseProductTitle,
    string Description,
    decimal? Budget,
    DateTime? DesiredDeadline,
    decimal? QuotedPrice,
    int? EstimatedDays,
    string? ArtisanComment,
    string Status,
    DateTime CreatedAt);

public class CreateCustomOrderDto
{
    [Range(1, int.MaxValue)]
    public int BuyerId { get; set; }

    [Range(1, int.MaxValue)]
    public int ArtisanId { get; set; }

    /// <summary>Виріб-зразок (необов'язково).</summary>
    public int? BaseProductId { get; set; }

    [Required, StringLength(4000, MinimumLength = 20)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 1_000_000)]
    public decimal? Budget { get; set; }

    public DateTime? DesiredDeadline { get; set; }
}

public class QuoteCustomOrderDto
{
    [Range(1, int.MaxValue)]
    public int ArtisanId { get; set; }

    [Range(1, 1_000_000)]
    public decimal Price { get; set; }

    [Range(1, 365)]
    public int EstimatedDays { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }
}

/// <summary>Дія над кастомним замовленням від імені користувача (прийняти, скасувати, почати…).</summary>
public class CustomOrderActionDto
{
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }
}

// ---------- Відгуки ----------

public record ReviewDto(int Id, int ProductId, int BuyerId, string BuyerName, int Rating, string? Comment, DateTime CreatedAt);

public class CreateReviewDto
{
    [Range(1, int.MaxValue)]
    public int BuyerId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }
}
