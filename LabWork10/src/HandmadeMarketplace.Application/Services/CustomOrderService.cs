using HandmadeMarketplace.Application.Common;
using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Interfaces;
using HandmadeMarketplace.Application.Mapping;
using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Enums;
using HandmadeMarketplace.Domain.Interfaces;

namespace HandmadeMarketplace.Application.Services;

/// <summary>
/// Кастомні замовлення — ключова особливість маркетплейсу рукодільників.
/// Життєвий цикл: Pending → Quoted → Accepted → InProgress → Completed
///                (Pending/Quoted → Rejected майстром або Cancelled покупцем).
/// </summary>
public class CustomOrderService : ICustomOrderService
{
    private readonly ICustomOrderRepository _customOrders;
    private readonly IUserRepository _users;
    private readonly IProductRepository _products;
    private readonly IUnitOfWork _uow;

    public CustomOrderService(
        ICustomOrderRepository customOrders,
        IUserRepository users,
        IProductRepository products,
        IUnitOfWork uow)
    {
        _customOrders = customOrders;
        _users = users;
        _products = products;
        _uow = uow;
    }

    public async Task<CustomOrderDto> CreateAsync(CreateCustomOrderDto dto, CancellationToken ct = default)
    {
        if (dto.BuyerId == dto.ArtisanId)
            throw new BusinessRuleException("Не можна створити замовлення самому собі.");

        var buyer = await _users.GetByIdAsync(dto.BuyerId, ct) ?? throw new NotFoundException("Покупця", dto.BuyerId);
        var artisan = await _users.GetByIdAsync(dto.ArtisanId, ct) ?? throw new NotFoundException("Майстра", dto.ArtisanId);

        if (!artisan.IsArtisan)
            throw new BusinessRuleException("Кастомне замовлення можна надіслати лише майстру.");

        if (dto.DesiredDeadline.HasValue && dto.DesiredDeadline.Value.Date <= DateTime.UtcNow.Date)
            throw new BusinessRuleException("Бажаний термін має бути в майбутньому.");

        Product? baseProduct = null;
        if (dto.BaseProductId.HasValue)
        {
            baseProduct = await _products.GetByIdAsync(dto.BaseProductId.Value, ct)
                          ?? throw new NotFoundException("Виріб", dto.BaseProductId.Value);

            if (baseProduct.ArtisanId != artisan.Id)
                throw new BusinessRuleException("Обраний виріб-зразок належить іншому майстру.");
            if (!baseProduct.IsCustomizable)
                throw new BusinessRuleException($"Майстер не приймає індивідуальні замовлення на \"{baseProduct.Title}\".");
        }

        var request = new CustomOrderRequest
        {
            BuyerId = buyer.Id,
            Buyer = buyer,
            ArtisanId = artisan.Id,
            Artisan = artisan,
            BaseProductId = baseProduct?.Id,
            BaseProduct = baseProduct,
            Description = dto.Description.Trim(),
            Budget = dto.Budget,
            DesiredDeadline = dto.DesiredDeadline,
            Status = CustomOrderStatus.Pending
        };

        await _customOrders.AddAsync(request, ct);
        await _uow.SaveChangesAsync(ct);
        return request.ToDto();
    }

    public async Task<CustomOrderDto> GetByIdAsync(int id, CancellationToken ct = default) =>
        (await LoadAsync(id, ct)).ToDto();

    public async Task<IReadOnlyList<CustomOrderDto>> GetForBuyerAsync(int buyerId, CancellationToken ct = default) =>
        (await _customOrders.GetByBuyerAsync(buyerId, ct)).Select(c => c.ToDto()).ToList();

    public async Task<IReadOnlyList<CustomOrderDto>> GetForArtisanAsync(int artisanId, CancellationToken ct = default) =>
        (await _customOrders.GetByArtisanAsync(artisanId, ct)).Select(c => c.ToDto()).ToList();

    // ---------- Дії майстра ----------

    public async Task<CustomOrderDto> QuoteAsync(int id, QuoteCustomOrderDto dto, CancellationToken ct = default)
    {
        var request = await LoadAsync(id, ct);
        EnsureArtisan(request, dto.ArtisanId);
        // Майстер може переоцінити замовлення, поки покупець його не прийняв
        EnsureStatus(request, CustomOrderStatus.Pending, CustomOrderStatus.Quoted);

        request.QuotedPrice = dto.Price;
        request.EstimatedDays = dto.EstimatedDays;
        request.ArtisanComment = dto.Comment;
        return await SetStatusAsync(request, CustomOrderStatus.Quoted, ct);
    }

    public async Task<CustomOrderDto> RejectAsync(int id, int artisanId, string? comment, CancellationToken ct = default)
    {
        var request = await LoadAsync(id, ct);
        EnsureArtisan(request, artisanId);
        EnsureStatus(request, CustomOrderStatus.Pending, CustomOrderStatus.Quoted);

        request.ArtisanComment = comment ?? request.ArtisanComment;
        return await SetStatusAsync(request, CustomOrderStatus.Rejected, ct);
    }

    public async Task<CustomOrderDto> StartAsync(int id, int artisanId, CancellationToken ct = default)
    {
        var request = await LoadAsync(id, ct);
        EnsureArtisan(request, artisanId);
        EnsureStatus(request, CustomOrderStatus.Accepted);
        return await SetStatusAsync(request, CustomOrderStatus.InProgress, ct);
    }

    public async Task<CustomOrderDto> CompleteAsync(int id, int artisanId, CancellationToken ct = default)
    {
        var request = await LoadAsync(id, ct);
        EnsureArtisan(request, artisanId);
        EnsureStatus(request, CustomOrderStatus.InProgress);
        return await SetStatusAsync(request, CustomOrderStatus.Completed, ct);
    }

    // ---------- Дії покупця ----------

    public async Task<CustomOrderDto> AcceptAsync(int id, int buyerId, CancellationToken ct = default)
    {
        var request = await LoadAsync(id, ct);
        EnsureBuyer(request, buyerId);
        EnsureStatus(request, CustomOrderStatus.Quoted);
        return await SetStatusAsync(request, CustomOrderStatus.Accepted, ct);
    }

    public async Task<CustomOrderDto> CancelAsync(int id, int buyerId, CancellationToken ct = default)
    {
        var request = await LoadAsync(id, ct);
        EnsureBuyer(request, buyerId);
        EnsureStatus(request, CustomOrderStatus.Pending, CustomOrderStatus.Quoted);
        return await SetStatusAsync(request, CustomOrderStatus.Cancelled, ct);
    }

    // ---------- Допоміжні методи ----------

    private async Task<CustomOrderRequest> LoadAsync(int id, CancellationToken ct) =>
        await _customOrders.GetWithDetailsAsync(id, ct) ?? throw new NotFoundException("Кастомне замовлення", id);

    private async Task<CustomOrderDto> SetStatusAsync(CustomOrderRequest request, CustomOrderStatus status, CancellationToken ct)
    {
        request.Status = status;
        request.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
        return request.ToDto();
    }

    private static void EnsureArtisan(CustomOrderRequest request, int artisanId)
    {
        if (request.ArtisanId != artisanId)
            throw new ForbiddenException("Цю дію може виконати лише майстер, якому адресовано замовлення.");
    }

    private static void EnsureBuyer(CustomOrderRequest request, int buyerId)
    {
        if (request.BuyerId != buyerId)
            throw new ForbiddenException("Цю дію може виконати лише автор замовлення.");
    }

    private static void EnsureStatus(CustomOrderRequest request, params CustomOrderStatus[] allowed)
    {
        if (!allowed.Contains(request.Status))
            throw new BusinessRuleException(
                $"Дія недоступна для замовлення у статусі {request.Status}. Очікується: {string.Join(", ", allowed)}.");
    }
}
