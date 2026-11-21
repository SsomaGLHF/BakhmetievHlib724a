using HandmadeMarketplace.Application.Common;
using HandmadeMarketplace.Application.Services;
using HandmadeMarketplace.Domain.Common;
using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Interfaces;
using Moq;

namespace HandmadeMarketplace.Tests;

public class CurrencyServiceTests
{
    private readonly Mock<IExchangeRateRepository> _rates = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly CurrencyService _sut;

    public CurrencyServiceTests() => _sut = new CurrencyService(_rates.Object, _products.Object);

    private static ExchangeRate Usd => new()
    {
        CurrencyCode = "USD", CurrencyName = "Долар США", Rate = 40m, Date = new DateOnly(2026, 11, 21)
    };

    [Fact]
    public async Task GetProductPrice_ConvertsUahToCurrency()
    {
        _products.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new Product { Id = 1, Title = "Глечик", Price = 850m });
        _rates.Setup(r => r.GetRateAsync("USD", It.IsAny<CancellationToken>())).ReturnsAsync(Usd);

        var result = await _sut.GetProductPriceAsync(1, "usd");

        Assert.Equal("USD", result.Currency);
        Assert.Equal(850m, result.PriceUah);
        Assert.Equal(21.25m, result.ConvertedPrice);   // 850 / 40
    }

    [Theory]
    [InlineData(1200, 44.9293, 26.71)]
    [InlineData(950, 52.1, 18.23)]
    [InlineData(100, 3, 33.33)]
    public void ConvertFromUah_RoundsToTwoDecimals(decimal uah, decimal rate, decimal expected) =>
        Assert.Equal(expected, CurrencyService.ConvertFromUah(uah, rate));

    [Fact]
    public void ConvertFromUah_ZeroRate_Throws() =>
        Assert.Throws<BusinessRuleException>(() => CurrencyService.ConvertFromUah(100m, 0m));

    [Theory]
    [InlineData("")]
    [InlineData("US")]
    [InlineData("US1")]
    [InlineData("DOLLAR")]
    public async Task GetRate_InvalidCode_ThrowsWithoutCallingApi(string code)
    {
        await Assert.ThrowsAsync<BusinessRuleException>(() => _sut.GetRateAsync(code));
        _rates.Verify(r => r.GetRateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetRate_UnknownCurrency_ThrowsNotFound()
    {
        _rates.Setup(r => r.GetRateAsync("XYZ", It.IsAny<CancellationToken>())).ReturnsAsync((ExchangeRate?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetRateAsync("XYZ"));
    }

    [Fact]
    public async Task GetProductPrice_ProductNotFound_DoesNotCallApi()
    {
        _products.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetProductPriceAsync(99, "USD"));
        _rates.Verify(r => r.GetRateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetCatalogPrices_CallsApiOnce_AndSkipsInactiveProducts()
    {
        _rates.Setup(r => r.GetRateAsync("USD", It.IsAny<CancellationToken>())).ReturnsAsync(Usd);
        _products.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Product>
        {
            new() { Id = 1, Title = "Глечик", Price = 850m },
            new() { Id = 2, Title = "Чашки", Price = 1200m },
            new() { Id = 3, Title = "Знятий з продажу", Price = 500m, IsActive = false }
        });

        var result = await _sut.GetCatalogPricesAsync("USD");

        Assert.Equal(2, result.Count);
        Assert.Equal(30m, result[1].ConvertedPrice);
        _rates.Verify(r => r.GetRateAsync("USD", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPopularRates_FiltersOnlyPopularCurrencies()
    {
        _rates.Setup(r => r.GetAllRatesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<ExchangeRate>
        {
            Usd,
            new() { CurrencyCode = "EUR", Rate = 48m },
            new() { CurrencyCode = "JPY", Rate = 0.27m },
            new() { CurrencyCode = "XAU", Rate = 170000m }
        });

        var result = await _sut.GetPopularRatesAsync();

        Assert.Equal(new[] { "USD", "EUR" }, result.Select(r => r.CurrencyCode));
    }

    [Fact]
    public async Task GetRate_ExternalApiFails_PropagatesExternalApiException()
    {
        _rates.Setup(r => r.GetRateAsync("USD", It.IsAny<CancellationToken>()))
              .ThrowsAsync(new ExternalApiException("Сервіс НБУ недоступний."));

        await Assert.ThrowsAsync<ExternalApiException>(() => _sut.GetRateAsync("USD"));
    }
}
