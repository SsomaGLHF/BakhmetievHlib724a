using HandmadeMarketplace.Domain.Common;
using HandmadeMarketplace.Domain.Enums;

namespace HandmadeMarketplace.Domain.Entities;

/// <summary>
/// Замовлення готових виробів із каталогу.
/// </summary>
public class Order : BaseEntity
{
    public int BuyerId { get; set; }
    public User Buyer { get; set; } = null!;

    public string ShippingAddress { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.New;

    /// <summary>Сума товарів без комісії.</summary>
    public decimal Subtotal { get; set; }

    /// <summary>Комісія платформи.</summary>
    public decimal PlatformFee { get; set; }

    public decimal Total { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

/// <summary>
/// Позиція замовлення. Ціна фіксується на момент покупки.
/// </summary>
public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}
