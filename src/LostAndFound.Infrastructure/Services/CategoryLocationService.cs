using LostAndFound.Domain.Entities;
using LostAndFound.Infrastructure.Data;
using LostAndFound.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<Result<CategoryDto>> GetByIdAsync(int id, CancellationToken ct = default);
}

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _db;

    public CategoryService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _db.Categories.OrderBy(c => c.Name).ToListAsync(ct)).Select(Map).ToList();

    public async Task<Result<CategoryDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var c = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id, ct);
        return c is null ? Result<CategoryDto>.Fail("Category not found", 404) : Result<CategoryDto>.Ok(Map(c));
    }

    private static CategoryDto Map(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Description = c.Description
    };
}

public interface ILocationService
{
    Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken ct = default);
    Task<Result<LocationDto>> GetByIdAsync(int id, CancellationToken ct = default);
}

public class LocationService : ILocationService
{
    private readonly ApplicationDbContext _db;

    public LocationService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _db.Locations.OrderBy(l => l.Name).ToListAsync(ct)).Select(Map).ToList();

    public async Task<Result<LocationDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var l = await _db.Locations.FirstOrDefaultAsync(x => x.Id == id, ct);
        return l is null ? Result<LocationDto>.Fail("Location not found", 404) : Result<LocationDto>.Ok(Map(l));
    }

    private static LocationDto Map(Location l) => new()
    {
        Id = l.Id,
        Name = l.Name,
        Building = l.Building,
        Latitude = l.Latitude,
        Longitude = l.Longitude
    };
}
