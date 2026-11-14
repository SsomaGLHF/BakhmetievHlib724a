using HandmadeMarketplace.Application.Common;
using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Services;
using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Enums;
using HandmadeMarketplace.Domain.Interfaces;
using Moq;

namespace HandmadeMarketplace.Tests;

public class CustomOrderServiceTests
{
    private readonly Mock<ICustomOrderRepository> _customOrders = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly CustomOrderService _sut;

    private readonly User _buyer = new() { Id = 4, FullName = "Тарас", Role = UserRole.Buyer };
    private readonly User _artisan = new() { Id = 1, FullName = "Олена", ShopName = "Глиняна хата", Role = UserRole.Artisan };

    public CustomOrderServiceTests()
    {
        _users.Setup(r => r.GetByIdAsync(_buyer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(_buyer);
        _users.Setup(r => r.GetByIdAsync(_artisan.Id, It.IsAny<CancellationToken>())).ReturnsAsync(_artisan);
        _sut = new CustomOrderService(_customOrders.Object, _users.Object, _products.Object, _uow.Object);
    }

    private CustomOrderRequest Existing(CustomOrderStatus status)
    {
        var request = new CustomOrderRequest
        {
            Id = 7, BuyerId = _buyer.Id, Buyer = _buyer, ArtisanId = _artisan.Id, Artisan = _artisan,
            Description = "Глечик на 2 літри з іменем", Status = status
        };
        _customOrders.Setup(r => r.GetWithDetailsAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(request);
        return request;
    }

    [Fact]
    public async Task Create_ToArtisan_StatusPending()
    {
        var result = await _sut.CreateAsync(new CreateCustomOrderDto
        {
            BuyerId = _buyer.Id, ArtisanId = _artisan.Id, Description = "Хочу глечик на 2 літри з іменем"
        });

        Assert.Equal("Pending", result.Status);
        Assert.Equal("Глиняна хата", result.ArtisanShopName);
    }

    [Fact]
    public async Task Create_ToNonArtisan_Throws()
    {
        var otherBuyer = new User { Id = 5, Role = UserRole.Buyer };
        _users.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(otherBuyer);

        await Assert.ThrowsAsync<BusinessRuleException>(() => _sut.CreateAsync(new CreateCustomOrderDto
        {
            BuyerId = _buyer.Id, ArtisanId = 5, Description = "Хочу глечик на 2 літри з іменем"
        }));
    }

    [Fact]
    public async Task Create_BaseProductNotCustomizable_Throws()
    {
        var product = new Product { Id = 4, Title = "Конструктор", ArtisanId = _artisan.Id, IsCustomizable = false };
        _products.Setup(r => r.GetByIdAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        await Assert.ThrowsAsync<BusinessRuleException>(() => _sut.CreateAsync(new CreateCustomOrderDto
        {
            BuyerId = _buyer.Id, ArtisanId = _artisan.Id, BaseProductId = 4, Description = "Конструктор на 50 деталей"
        }));
    }

    [Fact]
    public async Task FullLifecycle_PendingToCompleted()
    {
        var request = Existing(CustomOrderStatus.Pending);

        await _sut.QuoteAsync(7, new QuoteCustomOrderDto { ArtisanId = 1, Price = 1400m, EstimatedDays = 14 });
        Assert.Equal(CustomOrderStatus.Quoted, request.Status);
        Assert.Equal(1400m, request.QuotedPrice);

        await _sut.AcceptAsync(7, _buyer.Id);
        Assert.Equal(CustomOrderStatus.Accepted, request.Status);

        await _sut.StartAsync(7, _artisan.Id);
        Assert.Equal(CustomOrderStatus.InProgress, request.Status);

        var result = await _sut.CompleteAsync(7, _artisan.Id);
        Assert.Equal("Completed", result.Status);
    }

    [Fact]
    public async Task Quote_ByAnotherArtisan_Forbidden()
    {
        Existing(CustomOrderStatus.Pending);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _sut.QuoteAsync(7, new QuoteCustomOrderDto { ArtisanId = 2, Price = 100m, EstimatedDays = 3 }));
    }

    [Fact]
    public async Task Accept_WithoutQuote_Throws()
    {
        Existing(CustomOrderStatus.Pending);

        await Assert.ThrowsAsync<BusinessRuleException>(() => _sut.AcceptAsync(7, _buyer.Id));
    }

    [Fact]
    public async Task Cancel_AfterAccepted_Throws()
    {
        Existing(CustomOrderStatus.Accepted);

        await Assert.ThrowsAsync<BusinessRuleException>(() => _sut.CancelAsync(7, _buyer.Id));
    }
}
