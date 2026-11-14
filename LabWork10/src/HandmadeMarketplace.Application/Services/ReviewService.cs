using HandmadeMarketplace.Application.Common;
using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Interfaces;
using HandmadeMarketplace.Application.Mapping;
using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Interfaces;

namespace HandmadeMarketplace.Application.Services;

/// <summary>
/// Відгуки: залишити може лише покупець, який отримав виріб, і лише один раз.
/// </summary>
public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviews;
    private readonly IProductRepository _products;
    private readonly IUserRepository _users;
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;

    public ReviewService(
        IReviewRepository reviews,
        IProductRepository products,
        IUserRepository users,
        IOrderRepository orders,
        IUnitOfWork uow)
    {
        _reviews = reviews;
        _products = products;
        _users = users;
        _orders = orders;
        _uow = uow;
    }

    public async Task<IReadOnlyList<ReviewDto>> GetForProductAsync(int productId, CancellationToken ct = default)
    {
        if (!await _products.ExistsAsync(productId, ct))
            throw new NotFoundException("Виріб", productId);

        return (await _reviews.GetByProductAsync(productId, ct)).Select(r => r.ToDto()).ToList();
    }

    public async Task<ReviewDto> CreateAsync(int productId, CreateReviewDto dto, CancellationToken ct = default)
    {
        var product = await _products.GetByIdAsync(productId, ct) ?? throw new NotFoundException("Виріб", productId);
        var buyer = await _users.GetByIdAsync(dto.BuyerId, ct) ?? throw new NotFoundException("Покупця", dto.BuyerId);

        if (dto.Rating is < 1 or > 5)
            throw new BusinessRuleException("Оцінка має бути від 1 до 5.");

        if (!await _orders.HasDeliveredProductAsync(buyer.Id, product.Id, ct))
            throw new ForbiddenException("Залишити відгук можна лише після отримання цього виробу.");

        if (await _reviews.ExistsForBuyerAsync(buyer.Id, product.Id, ct))
            throw new ConflictException("Ви вже залишили відгук на цей виріб.");

        var review = new Review
        {
            ProductId = product.Id,
            BuyerId = buyer.Id,
            Buyer = buyer,
            Rating = dto.Rating,
            Comment = dto.Comment
        };

        // Інкрементально перераховуємо середній рейтинг виробу
        var total = product.AverageRating * product.ReviewsCount + dto.Rating;
        product.ReviewsCount += 1;
        product.AverageRating = Math.Round(total / product.ReviewsCount, 2);

        await _reviews.AddAsync(review, ct);
        await _uow.SaveChangesAsync(ct);
        return review.ToDto();
    }
}
