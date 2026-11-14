using HandmadeMarketplace.Domain.Common;

namespace HandmadeMarketplace.Domain.Entities;

/// <summary>
/// Виріб ручної роботи, виставлений майстром на продаж.
/// </summary>
public class Product : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }

    /// <summary>Кількість готових екземплярів у наявності.</summary>
    public int Stock { get; set; }

    /// <summary>Матеріали (наприклад: "льон, натуральний віск").</summary>
    public string? Materials { get; set; }

    /// <summary>Чи приймає майстер індивідуальні замовлення на основі цього виробу.</summary>
    public bool IsCustomizable { get; set; }

    /// <summary>Орієнтовний термін виготовлення під замовлення, днів.</summary>
    public int? ProductionDays { get; set; }

    public bool IsActive { get; set; } = true;

    public double AverageRating { get; set; }
    public int ReviewsCount { get; set; }

    public int ArtisanId { get; set; }
    public User Artisan { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>Списує товар зі складу під замовлення.</summary>
    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Кількість має бути більшою за нуль.");
        if (Stock < quantity)
            throw new DomainException($"Недостатньо товару \"{Title}\": в наявності {Stock}, запитано {quantity}.");
        Stock -= quantity;
    }

    /// <summary>Повертає товар на склад (наприклад, при скасуванні замовлення).</summary>
    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Кількість має бути більшою за нуль.");
        Stock += quantity;
    }
}
