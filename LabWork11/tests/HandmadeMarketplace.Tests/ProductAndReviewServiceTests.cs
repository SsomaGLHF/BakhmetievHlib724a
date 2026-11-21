using HandmadeMarketplace.Application.Common;
using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Services;
using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Enums;
using HandmadeMarketplace.Domain.Interfaces;
using Moq;

namespace HandmadeMarketplace.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<ICategoryRepository> _categories = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _users.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
              .ReturnsAsync(new User { Id = 1, Role = UserRole.Artisan, ShopName = "Глиняна хата" });
        _users.Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>()))
              .ReturnsAsync(new User { Id = 3, Role = UserRole.Buyer });
        _categories.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new Category { Id = 1, Name = "Кераміка" });

        _sut = new ProductService(_products.Object, _users.Object, _categories.Object, _uow.Object);
    }

    private static CreateProductDto Dto(int artisanId) => new()
    {
        ArtisanId = artisanId, CategoryId = 1, Title = "Тарілка", Price = 400m, Stock = 3,
        IsCustomizable = true, ProductionDays = 5
    };

    [Fact]
    public async Task Create_ByArtisan_Succeeds()
    {
        var result = await _sut.CreateAsync(Dto(1));

        Assert.Equal("Кераміка", result.CategoryName);
        Assert.Equal("Глиняна хата", result.ArtisanShopName);
        _products.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_ByBuyer_Forbidden()
    {
        await Assert.ThrowsAsync<ForbiddenException>(() => _sut.CreateAsync(Dto(3)));
    }

    [Fact]
    public async Task Create_CustomizableWithoutProductionDays_Throws()
    {
        var dto = Dto(1);
        dto.ProductionDays = null;
        await Assert.ThrowsAsync<BusinessRuleException>(() => _sut.CreateAsync(dto));
    }

    [Fact]
    public async Task Search_MinPriceGreaterThanMax_Throws()
    {
        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _sut.SearchAsync(new ProductQueryDto { MinPrice = 1000, MaxPrice = 100 }));
    }
}

public class ReviewServiceTests
{
    private readonly Mock<IReviewRepository> _reviews = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IOrderRepository> _orders = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly ReviewService _sut;
    private readonly Product _product = new() { Id = 1, Title = "Глечик", AverageRating = 4.0, ReviewsCount = 1 };

    public ReviewServiceTests()
    {
        _products.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(_product);
        _users.Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>()))
              .ReturnsAsync(new User { Id = 3, FullName = "Ірина", Role = UserRole.Buyer });
        _sut = new ReviewService(_reviews.Object, _products.Object, _users.Object, _orders.Object, _uow.Object);
    }

    [Fact]
    public async Task Create_AfterDelivery_UpdatesAverageRating()
    {
        _orders.Setup(r => r.HasDeliveredProductAsync(3, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await _sut.CreateAsync(1, new CreateReviewDto { BuyerId = 3, Rating = 5 });

        Assert.Equal(2, _product.ReviewsCount);
        Assert.Equal(4.5, _product.AverageRating);
    }

    [Fact]
    public async Task Create_WithoutPurchase_Forbidden()
    {
        _orders.Setup(r => r.HasDeliveredProductAsync(3, 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _sut.CreateAsync(1, new CreateReviewDto { BuyerId = 3, Rating = 5 }));
    }

    [Fact]
    public async Task Create_SecondReview_Conflict()
    {
        _orders.Setup(r => r.HasDeliveredProductAsync(3, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _reviews.Setup(r => r.ExistsForBuyerAsync(3, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() =>
            _sut.CreateAsync(1, new CreateReviewDto { BuyerId = 3, Rating = 4 }));
    }
}
