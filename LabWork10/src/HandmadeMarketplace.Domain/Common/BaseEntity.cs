namespace HandmadeMarketplace.Domain.Common;

/// <summary>
/// Базовий клас для всіх сутностей доменної області.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
