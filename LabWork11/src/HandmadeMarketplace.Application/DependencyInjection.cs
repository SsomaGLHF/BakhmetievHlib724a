using HandmadeMarketplace.Application.Interfaces;
using HandmadeMarketplace.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HandmadeMarketplace.Application;

/// <summary>
/// Реєстрація сервісів бізнес-логіки у DI-контейнері.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICustomOrderService, CustomOrderService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        return services;
    }
}
