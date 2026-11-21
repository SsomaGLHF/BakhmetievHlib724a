using HandmadeMarketplace.Application.Common;
using HandmadeMarketplace.Application.DTOs;
using HandmadeMarketplace.Application.Interfaces;
using HandmadeMarketplace.Application.Mapping;
using HandmadeMarketplace.Domain.Entities;
using HandmadeMarketplace.Domain.Enums;
using HandmadeMarketplace.Domain.Interfaces;

namespace HandmadeMarketplace.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public UserService(IUserRepository users, IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _users.GetAllAsync(ct)).Select(u => u.ToDto()).ToList();

    public async Task<IReadOnlyList<UserDto>> GetArtisansAsync(CancellationToken ct = default) =>
        (await _users.GetArtisansAsync(ct)).Select(u => u.ToDto()).ToList();

    public async Task<UserDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(id, ct) ?? throw new NotFoundException("Користувача", id);
        return user.ToDto();
    }

    public async Task<UserDto> RegisterAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _users.EmailExistsAsync(email, ct))
            throw new ConflictException($"Користувач з email {email} вже зареєстрований.");

        if (dto.Role == UserRole.Artisan && string.IsNullOrWhiteSpace(dto.ShopName))
            throw new BusinessRuleException("Майстер повинен вказати назву своєї майстерні.");

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            Role = dto.Role,
            ShopName = dto.Role == UserRole.Artisan ? dto.ShopName?.Trim() : null,
            Bio = dto.Bio
        };

        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
        return user.ToDto();
    }
}

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _uow;

    public CategoryService(ICategoryRepository categories, IUnitOfWork uow)
    {
        _categories = categories;
        _uow = uow;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _categories.GetAllAsync(ct)).Select(c => c.ToDto()).ToList();

    public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var category = await _categories.GetByIdAsync(id, ct) ?? throw new NotFoundException("Категорію", id);
        return category.ToDto();
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
    {
        var name = dto.Name.Trim();
        if (await _categories.NameExistsAsync(name, ct))
            throw new ConflictException($"Категорія \"{name}\" вже існує.");

        var category = new Category { Name = name, Description = dto.Description };
        await _categories.AddAsync(category, ct);
        await _uow.SaveChangesAsync(ct);
        return category.ToDto();
    }
}
