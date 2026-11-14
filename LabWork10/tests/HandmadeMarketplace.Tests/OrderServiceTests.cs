using HandmadeMarketplace.Application.Common;
using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Services;
using HandmadeMarketplace.Domain.Common;
using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Enums;
using HandmadeMarketplace.Domain.Interfaces;
using Moq;

namespace HandmadeMarketplace.Tests;

/// <summary>
/// Тести бізнес-логіки замовлень. Репозиторії замінені моками —
/// саме завдяки інтерфейсам рівень BLL тестується без бази даних.
/// </summary>
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orders = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly OrderService _sut;

    private readonly User _buyer = new() { Id = 10, FullName = "Ірина", Role = UserRole.Buyer };
    private readonly Product _jug = new() { Id = 1, Title = "Глечик", Price = 850m, Stock = 5, ArtisanId = 1, IsActive = true };
    private readonly Product _board = new() { Id = 2, Title = "Дошка", Price = 950m, Stock = 1, ArtisanId = 2, IsActive = true };

    public OrderServiceTests()
    {
        _users.Setup(r => r.GetByIdAsync(_buyer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(_buyer);
        _products.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((IEnumerable<int> ids, CancellationToken _) =>
                     new[] { _jug, _board }.Where(p => ids.Contains(p.Id)).ToList());

        _sut = new OrderService(_orders.Object, _products.Object, _users.Object, _uow.Object);
    }

    private CreateOrderDto Order(params (int productId, int qty)[] items) => new()
    {
        BuyerId = _buyer.Id,
        ShippingAddress = "м. Львів, вул. Городоцька, 10",
        Items = items.Select(i => new CreateOrderItemDto { ProductId = i.productId, Quantity = i.qty }).ToList()
    };

    [Fact]
    public async Task Create_CalculatesTotalsWithPlatformFee_AndDecreasesStock()
    {
        var result = await _sut.CreateAsync(Order((1, 2), (2, 1)));

        Assert.Equal(2650m, result.Subtotal);       // 850*2 + 950
        Assert.Equal(132.50m, result.PlatformFee);  // 5%
        Assert.Equal(2782.50m, result.Total);
        Assert.Equal(3, _jug.Stock);
        Assert.Equal(0, _board.Stock);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_MergesDuplicateItems()
    {
        var result = await _sut.CreateAsync(Order((1, 1), (1, 2)));

        Assert.Single(result.Items);
        Assert.Equal(3, result.Items[0].Quantity);
    }

    [Fact]
    public async Task Create_NotEnoughStock_ThrowsDomainException()
    {
        await Assert.ThrowsAsync<DomainException>(() => _sut.CreateAsync(Order((2, 2))));
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_ArtisanBuysOwnProduct_Throws()
    {
        var artisan = new User { Id = 1, Role = UserRole.Artisan };
        _users.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(artisan);

        var dto = Order((1, 1));
        dto.BuyerId = 1;

        await Assert.ThrowsAsync<BusinessRuleException>(() => _sut.CreateAsync(dto));
    }

    [Fact]
    public async Task Create_UnknownProduct_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateAsync(Order((999, 1))));
    }

    [Fact]
    public async Task ChangeStatus_Cancel_ReturnsStockToWarehouse()
    {
        var order = new Order { Id = 5, Status = OrderStatus.Paid };
        order.Items.Add(new OrderItem { ProductId = _jug.Id, Product = _jug, Quantity = 2, UnitPrice = 850m });
        _orders.Setup(r => r.GetWithItemsAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var result = await _sut.ChangeStatusAsync(5, OrderStatus.Cancelled);

        Assert.Equal("Cancelled", result.Status);
        Assert.Equal(7, _jug.Stock);
    }

    [Theory]
    [InlineData(OrderStatus.New, OrderStatus.Delivered)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Delivered, OrderStatus.New)]
    public async Task ChangeStatus_InvalidTransition_Throws(OrderStatus from, OrderStatus to)
    {
        var order = new Order { Id = 6, Status = from };
        _orders.Setup(r => r.GetWithItemsAsync(6, It.IsAny<CancellationToken>())).ReturnsAsync(order);

        await Assert.ThrowsAsync<BusinessRuleException>(() => _sut.ChangeStatusAsync(6, to));
    }
}
