namespace HandmadeMarketplace.Domain.Enums;

/// <summary>Роль користувача на маркетплейсі.</summary>
public enum UserRole
{
    Buyer = 0,    // покупець
    Artisan = 1,  // майстер-рукодільник (продавець)
    Admin = 2
}

/// <summary>Статус звичайного замовлення готових товарів.</summary>
public enum OrderStatus
{
    New = 0,
    Paid = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}

/// <summary>Статус кастомного (індивідуального) замовлення.</summary>
public enum CustomOrderStatus
{
    Pending = 0,     // покупець надіслав запит
    Quoted = 1,      // майстер запропонував ціну і термін
    Accepted = 2,    // покупець погодився
    InProgress = 3,  // майстер виготовляє виріб
    Completed = 4,   // виріб готовий / передано
    Rejected = 5,    // майстер відмовився
    Cancelled = 6    // покупець скасував
}
