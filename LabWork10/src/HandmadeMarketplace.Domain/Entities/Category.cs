using HandmadeMarketplace.Domain.Common;

namespace HandmadeMarketplace.Domain.Entities;

/// <summary>
/// Категорія виробів (кераміка, в'язання, прикраси, дерево тощо).
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
