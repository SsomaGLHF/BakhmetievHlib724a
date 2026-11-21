using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Common;
using HandmadeMarketplace.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace HandmadeMarketplace.Infrastructure.ExternalApis;

/// <summary>
/// Data Access Layer: виконує HTTP-запити до відкритого API Національного банку України
/// (https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange) та кешує відповіді.
/// </summary>
public class NbuExchangeRateRepository : IExchangeRateRepository
{
    private const string AllRatesCacheKey = "nbu:rates:all";

    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration;

    public NbuExchangeRateRepository(HttpClient httpClient, IMemoryCache cache, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _cache = cache;
        var minutes = int.TryParse(configuration["NbuApi:CacheMinutes"], out var m) ? m : 60;
        _cacheDuration = TimeSpan.FromMinutes(minutes);
    }

    public async Task<ExchangeRate?> GetRateAsync(string currencyCode, CancellationToken ct = default)
    {
        var code = currencyCode.Trim().ToUpperInvariant();
        var cacheKey = $"nbu:rate:{code}";

        if (_cache.TryGetValue(cacheKey, out ExchangeRate? cached))
            return cached;

        var items = await SendAsync($"exchange?valcode={Uri.EscapeDataString(code)}&json", ct);
        var rate = items.Select(Map).FirstOrDefault();

        // Кешуємо лише знайдені курси, щоб не «запам'ятати» помилку надовго
        if (rate is not null)
            _cache.Set(cacheKey, rate, _cacheDuration);

        return rate;
    }

    public async Task<IReadOnlyList<ExchangeRate>> GetAllRatesAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(AllRatesCacheKey, out IReadOnlyList<ExchangeRate>? cached) && cached is not null)
            return cached;

        var items = await SendAsync("exchange?json", ct);
        var rates = items.Select(Map).OrderBy(r => r.CurrencyCode).ToList();

        _cache.Set(AllRatesCacheKey, (IReadOnlyList<ExchangeRate>)rates, _cacheDuration);
        return rates;
    }

    private async Task<List<NbuRateDto>> SendAsync(string relativeUrl, CancellationToken ct)
    {
        try
        {
            using var response = await _httpClient.GetAsync(relativeUrl, ct);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                throw new ExternalApiException(
                    $"API НБУ повернуло помилку. Status: {(int)response.StatusCode}, Message: {body}");
            }

            return await response.Content.ReadFromJsonAsync<List<NbuRateDto>>(cancellationToken: ct)
                   ?? throw new ExternalApiException("API НБУ повернуло порожню відповідь.");
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalApiException("Сервіс НБУ недоступний.", ex);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            throw new ExternalApiException("Перевищено час очікування відповіді від API НБУ.", ex);
        }
        catch (System.Text.Json.JsonException ex)
        {
            throw new ExternalApiException("Некоректний формат відповіді API НБУ.", ex);
        }
    }

    private static ExchangeRate Map(NbuRateDto dto) => new()
    {
        CurrencyCode = dto.Cc,
        CurrencyName = dto.Txt,
        Rate = dto.Rate,
        Date = DateOnly.ParseExact(dto.ExchangeDate, "dd.MM.yyyy", CultureInfo.InvariantCulture)
    };

    /// <summary>Формат одного елемента JSON-відповіді НБУ.</summary>
    private sealed class NbuRateDto
    {
        [JsonPropertyName("r030")] public int NumericCode { get; set; }
        [JsonPropertyName("txt")] public string Txt { get; set; } = string.Empty;
        [JsonPropertyName("rate")] public decimal Rate { get; set; }
        [JsonPropertyName("cc")] public string Cc { get; set; } = string.Empty;
        [JsonPropertyName("exchangedate")] public string ExchangeDate { get; set; } = string.Empty;
    }
}
