using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Domain.Entities;

namespace HandmadeMarketplace.Application.Mapping;

/// <summary>
/// Ручне відображення сутностей домену у DTO (замість AutoMapper — без зайвих залежностей).
/// </summary>
public static class MappingExtensions
{
    public static UserDto ToDto(this User u) =>
        new(u.Id, u.FullName, u.Email, u.Role.ToString(), u.ShopName, u.Bio);

    public static CategoryDto ToDto(this Category c) =>
        new(c.Id, c.Name, c.Description);

    public static ProductDto ToDto(this Product p) =>
        new(p.Id, p.Title, p.Description, p.Price, p.Stock, p.Materials, p.IsCustomizable,
            p.ProductionDays, p.IsActive, p.AverageRating, p.ReviewsCount,
            p.CategoryId, p.Category?.Name ?? string.Empty,
            p.ArtisanId, p.Artisan?.ShopName ?? p.Artisan?.FullName ?? string.Empty);

    public static OrderDto ToDto(this Order o) =>
        new(o.Id, o.BuyerId, o.ShippingAddress, o.Status.ToString(),
            o.Subtotal, o.PlatformFee, o.Total, o.CreatedAt,
            o.Items.Select(i => new OrderItemDto(
                i.ProductId, i.Product?.Title ?? string.Empty, i.Quantity, i.UnitPrice, i.LineTotal)).ToList());

    public static CustomOrderDto ToDto(this CustomOrderRequest c) =>
        new(c.Id, c.BuyerId, c.Buyer?.FullName ?? string.Empty,
            c.ArtisanId, c.Artisan?.ShopName ?? c.Artisan?.FullName ?? string.Empty,
            c.BaseProductId, c.BaseProduct?.Title,
            c.Description, c.Budget, c.DesiredDeadline,
            c.QuotedPrice, c.EstimatedDays, c.ArtisanComment,
            c.Status.ToString(), c.CreatedAt);

    public static ReviewDto ToDto(this Review r) =>
        new(r.Id, r.ProductId, r.BuyerId, r.Buyer?.FullName ?? string.Empty, r.Rating, r.Comment, r.CreatedAt);
}
