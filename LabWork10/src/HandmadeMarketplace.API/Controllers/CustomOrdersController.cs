using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandmadeMarketplace.API.Controllers;

/// <summary>
/// Кастомні (індивідуальні) замовлення у майстрів.
/// Pending → Quoted → Accepted → InProgress → Completed.
/// </summary>
[ApiController]
[Route("api/custom-orders")]
public class CustomOrdersController : ControllerBase
{
    private readonly ICustomOrderService _service;

    public CustomOrdersController(ICustomOrderService service) => _service = service;

    /// <summary>Покупець надсилає запит майстру.</summary>
    [HttpPost]
    public async Task<ActionResult<CustomOrderDto>> Create(CreateCustomOrderDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomOrderDto>> GetById(int id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet("buyer/{buyerId:int}")]
    public async Task<ActionResult<IReadOnlyList<CustomOrderDto>>> GetForBuyer(int buyerId, CancellationToken ct) =>
        Ok(await _service.GetForBuyerAsync(buyerId, ct));

    [HttpGet("artisan/{artisanId:int}")]
    public async Task<ActionResult<IReadOnlyList<CustomOrderDto>>> GetForArtisan(int artisanId, CancellationToken ct) =>
        Ok(await _service.GetForArtisanAsync(artisanId, ct));

    /// <summary>Майстер пропонує ціну і термін.</summary>
    [HttpPost("{id:int}/quote")]
    public async Task<ActionResult<CustomOrderDto>> Quote(int id, QuoteCustomOrderDto dto, CancellationToken ct) =>
        Ok(await _service.QuoteAsync(id, dto, ct));

    /// <summary>Майстер відмовляється від замовлення.</summary>
    [HttpPost("{id:int}/reject")]
    public async Task<ActionResult<CustomOrderDto>> Reject(int id, CustomOrderActionDto dto, CancellationToken ct) =>
        Ok(await _service.RejectAsync(id, dto.UserId, dto.Comment, ct));

    /// <summary>Майстер розпочинає виготовлення.</summary>
    [HttpPost("{id:int}/start")]
    public async Task<ActionResult<CustomOrderDto>> Start(int id, CustomOrderActionDto dto, CancellationToken ct) =>
        Ok(await _service.StartAsync(id, dto.UserId, ct));

    /// <summary>Майстер завершує виготовлення.</summary>
    [HttpPost("{id:int}/complete")]
    public async Task<ActionResult<CustomOrderDto>> Complete(int id, CustomOrderActionDto dto, CancellationToken ct) =>
        Ok(await _service.CompleteAsync(id, dto.UserId, ct));

    /// <summary>Покупець приймає пропозицію майстра.</summary>
    [HttpPost("{id:int}/accept")]
    public async Task<ActionResult<CustomOrderDto>> Accept(int id, CustomOrderActionDto dto, CancellationToken ct) =>
        Ok(await _service.AcceptAsync(id, dto.UserId, ct));

    /// <summary>Покупець скасовує запит.</summary>
    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<CustomOrderDto>> Cancel(int id, CustomOrderActionDto dto, CancellationToken ct) =>
        Ok(await _service.CancelAsync(id, dto.UserId, ct));
}
