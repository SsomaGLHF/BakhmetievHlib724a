# Handmade Marketplace — маркетплейс для рукодільників

Лабораторна робота №11, варіант 2. Підключення ASP.NET Core проєкту до зовнішнього API
(на основі каркаса з ЛР10 — папка `LabWork10`). Звіт: `ЛР11 ПЗСК Бахметьєв ГД.docx`.

## ЛР11: зовнішній API НБУ — ціни у валюті покупця

Підключено відкритий API Національного банку України
(`https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange`) — офіційні курси валют.
API безкоштовне і не потребує ключа. Ціни виробів (у гривнях) перераховуються в USD, EUR, PLN, GBP.

| Шар | Що додано |
|---|---|
| Domain | `Entities/ExchangeRate`, `Interfaces/IExchangeRateRepository`, `Common/ExternalApiException` |
| Infrastructure (DAL) | `ExternalApis/NbuExchangeRateRepository` — типізований `HttpClient`, тайм-аут 10 с, кеш `IMemoryCache` 60 хв, обробка помилок |
| Application (BLL) | `Services/CurrencyService` — валідація коду валюти, перерахунок цін, один запит курсу на весь каталог |
| API | `Controllers/CurrencyController`; `ExceptionHandlingMiddleware` повертає 503, якщо НБУ недоступний |
| Tests | `CurrencyServiceTests` — 14 тестів (усього в рішенні 37) |

| Метод | Маршрут | Опис |
|---|---|---|
| GET | `/api/currency/rates` | Курси USD, EUR, PLN, GBP |
| GET | `/api/currency/rates/{code}` | Курс валюти за кодом ISO 4217 |
| GET | `/api/currency/products/{id}?currency=USD` | Ціна виробу у валюті |
| GET | `/api/currency/products?currency=EUR` | Каталог з цінами у валюті |

Налаштування — секція `NbuApi` в `appsettings.json` (`BaseUrl`, `CacheMinutes`).
Приклади запитів — у кінці `src/HandmadeMarketplace.API/HandmadeMarketplace.API.http`.

---

Нижче — опис каркаса з ЛР10.

## Структура рішення

```
HandmadeMarketplace.sln
├── src/
│   ├── HandmadeMarketplace.Domain          — сутності, enum-и, інтерфейси репозиторіїв (без залежностей)
│   ├── HandmadeMarketplace.Application     — DTO, інтерфейси та реалізації сервісів бізнес-логіки
│   ├── HandmadeMarketplace.Infrastructure  — EF Core: AppDbContext, конфігурації, репозиторії, UnitOfWork
│   └── HandmadeMarketplace.API             — контролери, middleware обробки помилок, Program.cs (DI)
└── tests/
    └── HandmadeMarketplace.Tests           — unit-тести сервісів (xUnit + Moq)
```

Залежності між проєктами (без циклів):

```
API ──► Application ──► Domain
 └────► Infrastructure ─┘
Tests ─► Application, Domain
```

## Запуск

1. Відкрити `HandmadeMarketplace.sln` у Visual Studio 2022 (потрібен .NET 8 SDK).
2. Зробити `HandmadeMarketplace.API` стартовим проєктом і натиснути F5.
3. Відкриється Swagger UI (`/swagger`). База SQLite `handmade_marketplace.db` створюється автоматично
   і заповнюється тестовими даними (2 майстри, 2 покупці, 4 категорії, 4 вироби).

Тести: **Test → Run All Tests** або `dotnet test`.

Приклади запитів — у файлі `src/HandmadeMarketplace.API/HandmadeMarketplace.API.http`.

### Перехід на SQL Server
Встановити пакет `Microsoft.EntityFrameworkCore.SqlServer`, у `Infrastructure/DependencyInjection.cs`
замінити `UseSqlite` на `UseSqlServer`, у `appsettings.json` вказати рядок підключення, напр.
`Server=(localdb)\\mssqllocaldb;Database=HandmadeMarketplace;Trusted_Connection=True;`.

## Основні можливості (API)

| Метод | Маршрут | Опис |
|---|---|---|
| GET | `/api/products` | Каталог з пошуком, фільтрами, сортуванням і пагінацією |
| POST/PUT/DELETE | `/api/products/{id}` | Майстер керує своїми виробами |
| GET/POST | `/api/products/{id}/reviews` | Відгуки (лише після доставки) |
| POST | `/api/orders` | Замовлення: перевірка залишків, списання, комісія 5% |
| PATCH | `/api/orders/{id}/status` | New → Paid → Shipped → Delivered / Cancelled |
| POST | `/api/custom-orders` | Кастомне замовлення у майстра |
| POST | `/api/custom-orders/{id}/quote` · `accept` · `start` · `complete` · `reject` · `cancel` | Життєвий цикл кастомного замовлення |
| GET/POST | `/api/users`, `/api/categories` | Користувачі та категорії |

> Автентифікацію не реалізовано (це каркас), тому ідентифікатор користувача передається в тілі запиту.
> У реальній системі він береться з JWT-токена.
