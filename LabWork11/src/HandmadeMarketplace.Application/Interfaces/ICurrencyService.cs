using HandmadeMarketplace.Application.DTOs;

namespace HandmadeMarketplace.Application.Interfaces;

/// <summary>Курси валют НБУ та перерахунок цін виробів у валюту покупця.</summary>
public interface ICurrencyService
{
    Task<ExchangeRateDto> GetRateAsync(string currencyCode, CancellationToken ct = default);
    Task<IReadOnlyList<ExchangeRateDto>> GetPopularRatesAsync(CancellationToken ct = default);
    Task<ProductPriceDto> GetProductPriceAsync(int productId, string currencyCode, CancellationToken ct = default);
    Task<IReadOnlyList<ProductPriceDto>> GetCatalogPricesAsync(string currencyCode, CancellationToken ct = default);
}
