namespace HandmadeMarketplace.Application.Common;

/// <summary>Сутність не знайдено (HTTP 404).</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entity, object key)
        : base($"{entity} з ідентифікатором {key} не знайдено.") { }
}

/// <summary>Порушено бізнес-правило (HTTP 400).</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>Користувач не має права на дію (HTTP 403).</summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}

/// <summary>Конфлікт даних, наприклад дубль email (HTTP 409).</summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

/// <summary>Сторінка результатів для пагінації.</summary>
public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

/// <summary>Константи бізнес-правил маркетплейсу.</summary>
public static class BusinessRules
{
    /// <summary>Комісія платформи з кожного замовлення (5%).</summary>
    public const decimal PlatformFeeRate = 0.05m;

    /// <summary>Мінімальна сума замовлення.</summary>
    public const decimal MinOrderAmount = 100m;

    /// <summary>Максимальна кількість одного товару в замовленні.</summary>
    public const int MaxQuantityPerItem = 10;
}
