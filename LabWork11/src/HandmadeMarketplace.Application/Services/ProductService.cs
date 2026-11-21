using HandmadeMarketplace.Application.Common;
using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Interfaces;
using HandmadeMarketplace.Application.Mapping;
using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Filters;
using HandmadeMarketplace.Domain.Interfaces;

namespace HandmadeMarketplace.Application.Services;

/// <summary>
/// Бізнес-логіка каталогу виробів ручної роботи.
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _products;
    private readonly IUserRepository _users;
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _uow;

    public ProductService(
        IProductRepository products,
        IUserRepository users,
        ICategoryRepository categories,
        IUnitOfWork uow)
    {
        _products = products;
        _users = users;
        _categories = categories;
        _uow = uow;
    }

    public async Task<PagedResult<ProductDto>> SearchAsync(ProductQueryDto query, CancellationToken ct = default)
    {
        if (query.MinPrice.HasValue && query.MaxPrice.HasValue && query.MinPrice > query.MaxPrice)
            throw new BusinessRuleException("Мінімальна ціна не може бути більшою за максимальну.");

        var filter = new ProductFilter
        {
            Search = query.Search,
            CategoryId = query.CategoryId,
            ArtisanId = query.ArtisanId,
            MinPrice = query.MinPrice,
            MaxPrice = query.MaxPrice,
            OnlyCustomizable = query.OnlyCustomizable,
            OnlyInStock = query.OnlyInStock,
            SortBy = query.SortBy,
            Page = Math.Max(1, query.Page),
            PageSize = Math.Clamp(query.PageSize, 1, 100)
        };

        var (items, total) = await _products.SearchAsync(filter, ct);
        return new PagedResult<ProductDto>(items.Select(p => p.ToDto()).ToList(), total, filter.Page, filter.PageSize);
    }

    public async Task<ProductDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var product = await _products.GetWithDetailsAsync(id, ct) ?? throw new NotFoundException("Виріб", id);
        return product.ToDto();
    }

    public async Task<IReadOnlyList<ProductDto>> GetByArtisanAsync(int artisanId, CancellationToken ct = default)
    {
        if (!await _users.ExistsAsync(artisanId, ct))
            throw new NotFoundException("Майстра", artisanId);

        return (await _products.GetByArtisanAsync(artisanId, ct)).Select(p => p.ToDto()).ToList();
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        var artisan = await _users.GetByIdAsync(dto.ArtisanId, ct)
                      ?? throw new NotFoundException("Майстра", dto.ArtisanId);
        if (!artisan.IsArtisan)
            throw new ForbiddenException("Виставляти вироби на продаж можуть лише майстри.");

        var category = await _categories.GetByIdAsync(dto.CategoryId, ct)
                       ?? throw new NotFoundException("Категорію", dto.CategoryId);

        ValidateCustomization(dto.IsCustomizable, dto.ProductionDays);

        var product = new Product
        {
            Title = dto.Title.Trim(),
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            Materials = dto.Materials,
            IsCustomizable = dto.IsCustomizable,
            ProductionDays = dto.ProductionDays,
            ArtisanId = artisan.Id,
            Artisan = artisan,
            CategoryId = category.Id,
            Category = category
        };

        await _products.AddAsync(product, ct);
        await _uow.SaveChangesAsync(ct);
        return product.ToDto();
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var product = await _products.GetWithDetailsAsync(id, ct) ?? throw new NotFoundException("Виріб", id);
        EnsureOwner(product, dto.ArtisanId);

        if (product.CategoryId != dto.CategoryId)
        {
            var category = await _categories.GetByIdAsync(dto.CategoryId, ct)
                           ?? throw new NotFoundException("Категорію", dto.CategoryId);
            product.CategoryId = category.Id;
            product.Category = category;
        }

        ValidateCustomization(dto.IsCustomizable, dto.ProductionDays);

        product.Title = dto.Title.Trim();
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.Materials = dto.Materials;
        product.IsCustomizable = dto.IsCustomizable;
        product.ProductionDays = dto.ProductionDays;
        product.IsActive = dto.IsActive;

        await _uow.SaveChangesAsync(ct);
        return product.ToDto();
    }

    public async Task DeactivateAsync(int id, int artisanId, CancellationToken ct = default)
    {
        var product = await _products.GetByIdAsync(id, ct) ?? throw new NotFoundException("Виріб", id);
        EnsureOwner(product, artisanId);

        // М'яке видалення: виріб зникає з каталогу, але залишається в історії замовлень
        product.IsActive = false;
        await _uow.SaveChangesAsync(ct);
    }

    private static void EnsureOwner(Product product, int artisanId)
    {
        if (product.ArtisanId != artisanId)
            throw new ForbiddenException("Редагувати виріб може лише майстер, який його створив.");
    }

    private static void ValidateCustomization(bool isCustomizable, int? productionDays)
    {
        if (isCustomizable && productionDays is null)
            throw new BusinessRuleException("Для виробу під замовлення потрібно вказати термін виготовлення.");
    }
}
