using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandmadeMarketplace.API.Controllers;

/// <summary>Користувачі: покупці та майстри.</summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    /// <summary>Усі користувачі.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken ct) =>
        Ok(await _userService.GetAllAsync(ct));

    /// <summary>Список майстрів (майстерень).</summary>
    [HttpGet("artisans")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetArtisans(CancellationToken ct) =>
        Ok(await _userService.GetArtisansAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken ct) =>
        Ok(await _userService.GetByIdAsync(id, ct));

    /// <summary>Реєстрація покупця або майстра.</summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Register(CreateUserDto dto, CancellationToken ct)
    {
        var user = await _userService.RegisterAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }
}

/// <summary>Категорії виробів.</summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken ct) =>
        Ok(await _categoryService.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken ct) =>
        Ok(await _categoryService.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto dto, CancellationToken ct)
    {
        var category = await _categoryService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }
}
