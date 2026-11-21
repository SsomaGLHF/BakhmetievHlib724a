namespace HandmadeMarketplace.Domain.Common;

/// <summary>
/// Помилка звернення до зовнішнього API: сервіс недоступний, тайм-аут або некоректна відповідь (HTTP 503).
/// </summary>
public class ExternalApiException : Exception
{
    public ExternalApiException(string message, Exception? innerException = null)
        : base(message, innerException) { }
}
