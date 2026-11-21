namespace HandmadeMarketplace.Application.DTOs;

// ---------- Курси валют (ЛР11, зовнішній API НБУ) ----------

public record ExchangeRateDto(string CurrencyCode, string CurrencyName, decimal Rate, DateOnly Date);

/// <summary>Ціна виробу в гривнях і перерахована в іноземну валюту за курсом НБУ.</summary>
public record ProductPriceDto(
    int ProductId,
    string Title,
    decimal PriceUah,
    string Currency,
    decimal ExchangeRate,
    decimal ConvertedPrice,
    DateOnly RateDate);
