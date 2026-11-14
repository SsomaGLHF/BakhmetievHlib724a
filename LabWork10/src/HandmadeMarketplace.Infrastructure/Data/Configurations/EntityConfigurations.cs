using HandmadeMarketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HandmadeMarketplace.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.Property(u => u.FullName).IsRequired().HasMaxLength(150);
        b.Property(u => u.Email).IsRequired().HasMaxLength(200);
        b.HasIndex(u => u.Email).IsUnique();
        b.Property(u => u.ShopName).HasMaxLength(150);
        b.Property(u => u.Bio).HasMaxLength(2000);
        b.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        b.Ignore(u => u.IsArtisan);
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.Property(c => c.Name).IsRequired().HasMaxLength(100);
        b.HasIndex(c => c.Name).IsUnique();
        b.Property(c => c.Description).HasMaxLength(500);
    }
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.Property(p => p.Title).IsRequired().HasMaxLength(200);
        b.Property(p => p.Description).HasMaxLength(4000);
        b.Property(p => p.Materials).HasMaxLength(300);
        b.Property(p => p.Price).HasPrecision(18, 2);

        b.HasOne(p => p.Artisan)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.ArtisanId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(p => p.Title);
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(500);
        b.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(o => o.Subtotal).HasPrecision(18, 2);
        b.Property(o => o.PlatformFee).HasPrecision(18, 2);
        b.Property(o => o.Total).HasPrecision(18, 2);

        b.HasOne(o => o.Buyer)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> b)
    {
        b.Property(i => i.UnitPrice).HasPrecision(18, 2);
        b.Ignore(i => i.LineTotal);

        b.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CustomOrderConfiguration : IEntityTypeConfiguration<CustomOrderRequest>
{
    public void Configure(EntityTypeBuilder<CustomOrderRequest> b)
    {
        b.ToTable("CustomOrders");
        b.Property(c => c.Description).IsRequired().HasMaxLength(4000);
        b.Property(c => c.ArtisanComment).HasMaxLength(2000);
        b.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(c => c.Budget).HasPrecision(18, 2);
        b.Property(c => c.QuotedPrice).HasPrecision(18, 2);

        // Два зовнішні ключі на таблицю Users — без каскадного видалення
        b.HasOne(c => c.Buyer)
            .WithMany()
            .HasForeignKey(c => c.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(c => c.Artisan)
            .WithMany()
            .HasForeignKey(c => c.ArtisanId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(c => c.BaseProduct)
            .WithMany()
            .HasForeignKey(c => c.BaseProductId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> b)
    {
        b.Property(r => r.Comment).HasMaxLength(2000);

        b.HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(r => r.Buyer)
            .WithMany()
            .HasForeignKey(r => r.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Один покупець — один відгук на виріб
        b.HasIndex(r => new { r.ProductId, r.BuyerId }).IsUnique();
    }
}
