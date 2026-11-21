using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandmadeMarketplace.API.Controllers;

/// <summary>
/// Курси валют НБУ (зовнішній API) та ціни виробів в іноземній валюті.
/// Помилки (400/404/503) перетворює на ProblemDetails ExceptionHandlingMiddleware.
/// </summary>
[ApiController]
[Route("api/currency")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyService _currencyService;

    public CurrencyController(ICurrencyService currencyService) => _currencyService = currencyService;

    /// <summary>Курси популярних валют (USD, EUR, PLN, GBP).</summary>
    [HttpGet("rates")]
    public async Task<ActionResult<IReadOnlyList<ExchangeRateDto>>> GetPopularRates(CancellationToken ct) =>
        Ok(await _currencyService.GetPopularRatesAsync(ct));

    /// <summary>Курс конкретної валюти за кодом ISO 4217, напр. USD.</summary>
    [HttpGet("rates/{code}")]
    public async Task<ActionResult<ExchangeRateDto>> GetRate(string code, CancellationToken ct) =>
        Ok(await _currencyService.GetRateAsync(code, ct));

    /// <summary>Ціна виробу у вибраній валюті.</summary>
    [HttpGet("products/{productId:int}")]
    public async Task<ActionResult<ProductPriceDto>> GetProductPrice(int productId, [FromQuery] string currency = "USD",
                                                                    CancellationToken ct = default) =>
        Ok(await _currencyService.GetProductPriceAsync(productId, currency, ct));

    /// <summary>Активні вироби каталогу з цінами у вибраній валюті.</summary>
    [HttpGet("products")]
    public async Task<ActionResult<IReadOnlyList<ProductPriceDto>>> GetCatalogPrices([FromQuery] string currency = "USD",
                                                                                     CancellationToken ct = default) =>
        Ok(await _currencyService.GetCatalogPricesAsync(currency, ct));
}
