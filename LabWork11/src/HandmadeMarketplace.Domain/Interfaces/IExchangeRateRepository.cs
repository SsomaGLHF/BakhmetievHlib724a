using HandmadeMarketplace.Domain.Entities;

namespace HandmadeMarketplace.Domain.Interfaces;

/// <summary>
/// Джерело курсів валют. Реалізується в Infrastructure через зовнішній API НБУ.
/// </summary>
public interface IExchangeRateRepository
{
    /// <summary>Курс однієї валюти; null, якщо такої валюти немає.</summary>
    Task<ExchangeRate?> GetRateAsync(string currencyCode, CancellationToken ct = default);

    /// <summary>Курси всіх валют на поточну дату.</summary>
    Task<IReadOnlyList<ExchangeRate>> GetAllRatesAsync(CancellationToken ct = default);
}
