using HandmadeMarketplace.Application.Common;
using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Interfaces;
using HandmadeMarketplace.Application.Mapping;
using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Enums;
using HandmadeMarketplace.Domain.Interfaces;

namespace HandmadeMarketplace.Application.Services;

/// <summary>
/// Оформлення замовлень готових виробів: перевірка наявності, списання зі складу,
/// розрахунок комісії, керування статусами.
/// </summary>
public class OrderService : IOrderService
{
    /// <summary>Дозволені переходи між статусами замовлення.</summary>
    private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
    {
        [OrderStatus.New] = new[] { OrderStatus.Paid, OrderStatus.Cancelled },
        [OrderStatus.Paid] = new[] { OrderStatus.Shipped, OrderStatus.Cancelled },
        [OrderStatus.Shipped] = new[] { OrderStatus.Delivered },
        [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
        [OrderStatus.Cancelled] = Array.Empty<OrderStatus>()
    };

    private readonly IOrderRepository _orders;
    private readonly IProductRepository _products;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public OrderService(
        IOrderRepository orders,
        IProductRepository products,
        IUserRepository users,
        IUnitOfWork uow)
    {
        _orders = orders;
        _products = products;
        _users = users;
        _uow = uow;
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        if (dto.Items.Count == 0)
            throw new BusinessRuleException("Замовлення має містити хоча б один товар.");

        var buyer = await _users.GetByIdAsync(dto.BuyerId, ct)
                    ?? throw new NotFoundException("Покупця", dto.BuyerId);

        // Об'єднуємо однакові товари в одну позицію
        var requested = dto.Items
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(i => i.Quantity) })
            .ToList();

        var products = (await _products.GetByIdsAsync(requested.Select(r => r.ProductId), ct))
            .ToDictionary(p => p.Id);

        var order = new Order
        {
            BuyerId = buyer.Id,
            ShippingAddress = dto.ShippingAddress.Trim(),
            Status = OrderStatus.New
        };

        foreach (var item in requested)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
                throw new NotFoundException("Виріб", item.ProductId);

            if (!product.IsActive)
                throw new BusinessRuleException($"Виріб \"{product.Title}\" знято з продажу.");

            if (product.ArtisanId == buyer.Id)
                throw new BusinessRuleException("Майстер не може купувати власні вироби.");

            if (item.Quantity > BusinessRules.MaxQuantityPerItem)
                throw new BusinessRuleException(
                    $"Не більше {BusinessRules.MaxQuantityPerItem} шт. одного виробу в замовленні.");

            product.DecreaseStock(item.Quantity); // кидає DomainException, якщо не вистачає

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = item.Quantity,
                UnitPrice = product.Price // фіксуємо ціну на момент покупки
            });
        }

        order.Subtotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
        if (order.Subtotal < BusinessRules.MinOrderAmount)
            throw new BusinessRuleException($"Мінімальна сума замовлення — {BusinessRules.MinOrderAmount} грн.");

        order.PlatformFee = Math.Round(order.Subtotal * BusinessRules.PlatformFeeRate, 2, MidpointRounding.AwayFromZero);
        order.Total = order.Subtotal + order.PlatformFee;

        await _orders.AddAsync(order, ct);
        await _uow.SaveChangesAsync(ct); // замовлення + списання залишків в одній транзакції
        return order.ToDto();
    }

    public async Task<OrderDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var order = await _orders.GetWithItemsAsync(id, ct) ?? throw new NotFoundException("Замовлення", id);
        return order.ToDto();
    }

    public async Task<IReadOnlyList<OrderDto>> GetByBuyerAsync(int buyerId, CancellationToken ct = default) =>
        (await _orders.GetByBuyerAsync(buyerId, ct)).Select(o => o.ToDto()).ToList();

    public async Task<OrderDto> ChangeStatusAsync(int id, OrderStatus newStatus, CancellationToken ct = default)
    {
        var order = await _orders.GetWithItemsAsync(id, ct) ?? throw new NotFoundException("Замовлення", id);

        if (!AllowedTransitions[order.Status].Contains(newStatus))
            throw new BusinessRuleException($"Неможливо змінити статус з {order.Status} на {newStatus}.");

        if (newStatus == OrderStatus.Cancelled)
        {
            // Повертаємо товари на склад
            foreach (var item in order.Items)
                item.Product.IncreaseStock(item.Quantity);
        }

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(ct);
        return order.ToDto();
    }
}
