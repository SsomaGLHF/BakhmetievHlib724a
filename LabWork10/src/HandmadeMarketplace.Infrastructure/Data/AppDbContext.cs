using HandmadeMarketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HandmadeMarketplace.Infrastructure.Data;

/// <summary>
/// Контекст бази даних маркетплейсу.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<CustomOrderRequest> CustomOrders => Set<CustomOrderRequest>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Підхоплює всі класи IEntityTypeConfiguration<T> з папки Configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // SQLite не підтримує тип decimal нативно (не вміє порівнювати й сортувати),
        // тому грошові значення зберігаємо як REAL. Для SQL Server цей рядок можна прибрати.
        if (Database.IsSqlite())
        {
            configurationBuilder.Properties<decimal>().HaveConversion<double>();
        }
    }
}
