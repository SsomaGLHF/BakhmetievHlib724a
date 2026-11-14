namespace HandmadeMarketplace.Domain.Filters;

/// <summary>
/// Параметри пошуку виробів у каталозі.
/// </summary>
public class ProductFilter
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public int? ArtisanId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? OnlyCustomizable { get; set; }
    public bool OnlyInStock { get; set; }

    /// <summary>newest | price_asc | price_desc | rating</summary>
    public string? SortBy { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
