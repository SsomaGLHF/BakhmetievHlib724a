using HandmadeMarketplace.Application.Common;
using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Interfaces;
using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Interfaces;

namespace HandmadeMarketplace.Application.Services;

/// <summary>
/// Business Logic Layer: перерахунок цін виробів у валюту покупця
/// за офіційним курсом НБУ (дані зовнішнього API).
/// </summary>
public class CurrencyService : ICurrencyService
{
    /// <summary>Валюти, у яких маркетплейс показує ціни іноземним покупцям.</summary>
    public static readonly string[] PopularCurrencies = { "USD", "EUR", "PLN", "GBP" };

    private readonly IExchangeRateRepository _exchangeRates;
    private readonly IProductRepository _products;

    public CurrencyService(IExchangeRateRepository exchangeRates, IProductRepository products)
    {
        _exchangeRates = exchangeRates;
        _products = products;
    }

    public async Task<ExchangeRateDto> GetRateAsync(string currencyCode, CancellationToken ct = default) =>
        ToDto(await FindRateAsync(currencyCode, ct));

    public async Task<IReadOnlyList<ExchangeRateDto>> GetPopularRatesAsync(CancellationToken ct = default)
    {
        var all = await _exchangeRates.GetAllRatesAsync(ct);
        return all.Where(r => PopularCurrencies.Contains(r.CurrencyCode)).Select(ToDto).ToList();
    }

    public async Task<ProductPriceDto> GetProductPriceAsync(int productId, string currencyCode, CancellationToken ct = default)
    {
        var product = await _products.GetByIdAsync(productId, ct)
                      ?? throw new NotFoundException("Виріб", productId);

        var rate = await FindRateAsync(currencyCode, ct);
        return Convert(product, rate);
    }

    public async Task<IReadOnlyList<ProductPriceDto>> GetCatalogPricesAsync(string currencyCode, CancellationToken ct = default)
    {
        var rate = await FindRateAsync(currencyCode, ct);   // один запит до API на весь каталог
        var products = await _products.GetAllAsync(ct);
        return products.Where(p => p.IsActive).Select(p => Convert(p, rate)).ToList();
    }

    /// <summary>Ціна у валюті = ціна в грн / курс, з округленням до копійок (центів).</summary>
    public static decimal ConvertFromUah(decimal priceUah, decimal rate)
    {
        if (rate <= 0)
            throw new BusinessRuleException("Курс валюти має бути більшим за нуль.");
        return Math.Round(priceUah / rate, 2, MidpointRounding.AwayFromZero);
    }

    private async Task<ExchangeRate> FindRateAsync(string currencyCode, CancellationToken ct)
    {
        var code = NormalizeCode(currencyCode);
        return await _exchangeRates.GetRateAsync(code, ct)
               ?? throw new NotFoundException("Курс валюти", code);
    }

    private static string NormalizeCode(string? currencyCode)
    {
        var code = (currencyCode ?? string.Empty).Trim().ToUpperInvariant();
        if (code.Length != 3 || !code.All(c => c is >= 'A' and <= 'Z'))
            throw new BusinessRuleException("Код валюти має складатися з 3 латинських літер (ISO 4217), напр. USD.");
        return code;
    }

    private static ProductPriceDto Convert(Product product, ExchangeRate rate) => new(
        product.Id, product.Title, product.Price, rate.CurrencyCode, rate.Rate,
        ConvertFromUah(product.Price, rate.Rate), rate.Date);

    private static ExchangeRateDto ToDto(ExchangeRate r) => new(r.CurrencyCode, r.CurrencyName, r.Rate, r.Date);
}
