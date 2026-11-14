using System.Reflection;
using System.Text.Json.Serialization;
using HandmadeMarketplace.API.Middleware;
using HandmadeMarketplace.Application;
using HandmadeMarketplace.Infrastructure;
using HandmadeMarketplace.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ---------- Presentation Layer ----------
builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Handmade Marketplace API", Version = "v1",
        Description = "Маркетплейс для рукодільників: каталог виробів, замовлення, кастомні замовлення, відгуки." });

    var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xml)) c.IncludeXmlComments(xml);
});

// ---------- Business Logic Layer + Data Access Layer ----------
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Створення БД і тестові дані
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
