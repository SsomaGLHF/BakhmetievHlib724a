using HandmadeMarketplace.Application.Common;
using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandmadeMarketplace.API.Controllers;

/// <summary>Каталог виробів ручної роботи та відгуки до них.</summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IReviewService _reviewService;

    public ProductsController(IProductService productService, IReviewService reviewService)
    {
        _productService = productService;
        _reviewService = reviewService;
    }

    /// <summary>
    /// Пошук у каталозі з фільтрами, сортуванням (newest | price_asc | price_desc | rating) і пагінацією.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> Search([FromQuery] ProductQueryDto query, CancellationToken ct) =>
        Ok(await _productService.SearchAsync(query, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken ct) =>
        Ok(await _productService.GetByIdAsync(id, ct));

    /// <summary>Усі вироби конкретного майстра (включно з неактивними).</summary>
    [HttpGet("artisan/{artisanId:int}")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetByArtisan(int artisanId, CancellationToken ct) =>
        Ok(await _productService.GetByArtisanAsync(artisanId, ct));

    /// <summary>Майстер виставляє новий виріб.</summary>
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto, CancellationToken ct)
    {
        var product = await _productService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(int id, UpdateProductDto dto, CancellationToken ct) =>
        Ok(await _productService.UpdateAsync(id, dto, ct));

    /// <summary>Зняти виріб з продажу (м'яке видалення).</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id, [FromQuery] int artisanId, CancellationToken ct)
    {
        await _productService.DeactivateAsync(id, artisanId, ct);
        return NoContent();
    }

    [HttpGet("{id:int}/reviews")]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetReviews(int id, CancellationToken ct) =>
        Ok(await _reviewService.GetForProductAsync(id, ct));

    /// <summary>Залишити відгук (лише після отримання виробу).</summary>
    [HttpPost("{id:int}/reviews")]
    public async Task<ActionResult<ReviewDto>> AddReview(int id, CreateReviewDto dto, CancellationToken ct)
    {
        var review = await _reviewService.CreateAsync(id, dto, ct);
        return CreatedAtAction(nameof(GetReviews), new { id }, review);
    }
}
