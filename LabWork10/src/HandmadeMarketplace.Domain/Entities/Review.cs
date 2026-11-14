using HandmadeMarketplace.Domain.Common;

namespace HandmadeMarketplace.Domain.Entities;

/// <summary>
/// Відгук покупця про виріб (лише після отримання замовлення).
/// </summary>
public class Review : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int BuyerId { get; set; }
    public User Buyer { get; set; } = null!;

    /// <summary>Оцінка від 1 до 5.</summary>
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
