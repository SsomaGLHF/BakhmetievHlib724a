using HandmadeMarketplace.Domain.Common;
using HandmadeMarketplace.Domain.Enums;

namespace HandmadeMarketplace.Domain.Entities;

/// <summary>
/// Кастомне замовлення: покупець описує бажаний виріб, майстер оцінює
/// вартість і термін, після погодження виготовляє його.
/// </summary>
public class CustomOrderRequest : BaseEntity
{
    public int BuyerId { get; set; }
    public User Buyer { get; set; } = null!;

    public int ArtisanId { get; set; }
    public User Artisan { get; set; } = null!;

    /// <summary>Виріб-зразок, на основі якого робиться замовлення (необов'язково).</summary>
    public int? BaseProductId { get; set; }
    public Product? BaseProduct { get; set; }

    public string Description { get; set; } = string.Empty;

    /// <summary>Бюджет, на який розраховує покупець.</summary>
    public decimal? Budget { get; set; }

    public DateTime? DesiredDeadline { get; set; }

    /// <summary>Ціна, яку запропонував майстер.</summary>
    public decimal? QuotedPrice { get; set; }

    /// <summary>Термін виготовлення, запропонований майстром (днів).</summary>
    public int? EstimatedDays { get; set; }

    public string? ArtisanComment { get; set; }

    public CustomOrderStatus Status { get; set; } = CustomOrderStatus.Pending;
    public DateTime? UpdatedAt { get; set; }
}
