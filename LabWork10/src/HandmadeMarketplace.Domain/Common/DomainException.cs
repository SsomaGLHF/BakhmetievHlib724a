namespace HandmadeMarketplace.Domain.Common;

/// <summary>
/// Порушення інваріанта доменної моделі (наприклад, недостатньо товару на складі).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
