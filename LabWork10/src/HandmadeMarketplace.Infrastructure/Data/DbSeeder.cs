using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HandmadeMarketplace.Infrastructure.Data;

/// <summary>
/// Створює БД (якщо її немає) і заповнює тестовими даними.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Users.AnyAsync())
            return;

        var ceramics = new Category { Name = "Кераміка", Description = "Посуд, вази, декор з глини" };
        var knitting = new Category { Name = "В'язання", Description = "Светри, шарфи, іграшки" };
        var jewelry = new Category { Name = "Прикраси", Description = "Сережки, браслети, намиста" };
        var wood = new Category { Name = "Вироби з дерева", Description = "Дошки, іграшки, декор" };

        var olena = new User
        {
            FullName = "Олена Коваль", Email = "olena@handmade.ua", Role = UserRole.Artisan,
            ShopName = "Глиняна хата", Bio = "Гончарка з Опішні, працюю з червоною глиною 12 років."
        };
        var andriy = new User
        {
            FullName = "Андрій Мельник", Email = "andriy@handmade.ua", Role = UserRole.Artisan,
            ShopName = "Дерев'яна майстерня", Bio = "Вироби з ясена, дуба та горіха."
        };
        var iryna = new User { FullName = "Ірина Шевчук", Email = "iryna@mail.ua", Role = UserRole.Buyer };
        var taras = new User { FullName = "Тарас Бондар", Email = "taras@mail.ua", Role = UserRole.Buyer };

        context.AddRange(ceramics, knitting, jewelry, wood, olena, andriy, iryna, taras);

        context.Products.AddRange(
            new Product
            {
                Title = "Глечик з петриківським розписом", Description = "Об'єм 1 л, покритий харчовою глазур'ю.",
                Price = 850m, Stock = 5, Materials = "червона глина, глазур", IsCustomizable = true, ProductionDays = 14,
                Artisan = olena, Category = ceramics
            },
            new Product
            {
                Title = "Набір чашок \"Ранок\" (2 шт.)", Description = "Чашки по 300 мл ручного формування.",
                Price = 1200m, Stock = 3, Materials = "біла глина", IsCustomizable = true, ProductionDays = 10,
                Artisan = olena, Category = ceramics
            },
            new Product
            {
                Title = "Обробна дошка з горіха", Description = "Просочена мінеральною олією, 40x25 см.",
                Price = 950m, Stock = 8, Materials = "волоський горіх, олія", IsCustomizable = true, ProductionDays = 7,
                Artisan = andriy, Category = wood
            },
            new Product
            {
                Title = "Дерев'яний конструктор для дітей", Description = "24 елементи, безпечні фарби.",
                Price = 1450m, Stock = 2, Materials = "бук, акрилові фарби", IsCustomizable = false,
                Artisan = andriy, Category = wood
            });

        await context.SaveChangesAsync();
    }
}
