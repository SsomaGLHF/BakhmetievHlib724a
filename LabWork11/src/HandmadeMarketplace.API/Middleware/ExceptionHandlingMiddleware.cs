using HandmadeMarketplace.Application.Common;
using HandmadeMarketplace.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace HandmadeMarketplace.API.Middleware;

/// <summary>
/// Перетворює винятки бізнес-логіки на HTTP-відповіді у форматі ProblemDetails,
/// щоб контролери не містили try/catch.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (status, title) = ex switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Не знайдено"),
                ForbiddenException => (StatusCodes.Status403Forbidden, "Доступ заборонено"),
                ConflictException => (StatusCodes.Status409Conflict, "Конфлікт"),
                BusinessRuleException or DomainException => (StatusCodes.Status400BadRequest, "Порушення бізнес-правила"),
                ExternalApiException => (StatusCodes.Status503ServiceUnavailable, "Зовнішній сервіс недоступний"),
                _ => (StatusCodes.Status500InternalServerError, "Внутрішня помилка сервера")
            };

            if (status == StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Необроблений виняток");

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == StatusCodes.Status500InternalServerError ? "Спробуйте пізніше." : ex.Message,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
