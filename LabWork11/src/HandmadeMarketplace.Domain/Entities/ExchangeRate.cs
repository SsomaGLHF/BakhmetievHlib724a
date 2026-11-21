namespace HandmadeMarketplace.Domain.Entities;

/// <summary>
/// Офіційний курс іноземної валюти до гривні (дані зовнішнього API НБУ).
/// Не зберігається в БД, тому не наслідує BaseEntity.
/// </summary>
public class ExchangeRate
{
    public string CurrencyCode { get; set; } = string.Empty;   // USD, EUR, PLN...
    public string CurrencyName { get; set; } = string.Empty;   // Долар США
    public decimal Rate { get; set; }                          // скільки грн коштує 1 одиниця валюти
    public DateOnly Date { get; set; }                         // дата, на яку встановлено курс
}
