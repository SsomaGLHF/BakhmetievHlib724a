using HandmadeMarketplace.Domain.Interfaces;
using HandmadeMarketplace.Infrastructure.Data;
using HandmadeMarketplace.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HandmadeMarketplace.Infrastructure;

/// <summary>
/// Реєстрація сервісів рівня доступу до даних у DI-контейнері.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? "Data Source=handmade_marketplace.db";

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        // Для SQL Server замініть рядок вище на:
        // services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomOrderRepository, CustomOrderRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        return services;
    }
}
