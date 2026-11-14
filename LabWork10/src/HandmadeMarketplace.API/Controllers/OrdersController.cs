using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandmadeMarketplace.API.Controllers;

/// <summary>Замовлення готових виробів.</summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService) => _orderService = orderService;

    /// <summary>Оформити замовлення: перевіряє наявність, списує товар, рахує комісію платформи.</summary>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto, CancellationToken ct)
    {
        var order = await _orderService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id, CancellationToken ct) =>
        Ok(await _orderService.GetByIdAsync(id, ct));

    [HttpGet("buyer/{buyerId:int}")]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetByBuyer(int buyerId, CancellationToken ct) =>
        Ok(await _orderService.GetByBuyerAsync(buyerId, ct));

    /// <summary>Змінити статус: New → Paid → Shipped → Delivered, або Cancelled (до відправлення).</summary>
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<OrderDto>> ChangeStatus(int id, UpdateOrderStatusDto dto, CancellationToken ct) =>
        Ok(await _orderService.ChangeStatusAsync(id, dto.Status, ct));
}
