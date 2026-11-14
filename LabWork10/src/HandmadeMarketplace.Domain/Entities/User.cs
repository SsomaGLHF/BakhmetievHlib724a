using HandmadeMarketplace.Domain.Common;
using HandmadeMarketplace.Domain.Enums;

namespace HandmadeMarketplace.Domain.Entities;

/// <summary>
/// Користувач платформи: покупець або майстер.
/// </summary>
public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Buyer;

    /// <summary>Назва майстерні (лише для майстрів).</summary>
    public string? ShopName { get; set; }

    /// <summary>Короткий опис майстра: техніки, матеріали, досвід.</summary>
    public string? Bio { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public bool IsArtisan => Role == UserRole.Artisan;
}
