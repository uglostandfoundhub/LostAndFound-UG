using LostAndFound.Domain.Entities;
using LostAndFound.Domain.Enums;
using LostAndFound.Infrastructure.Data;
using LostAndFound.Shared.Dtos;
using LostAndFound.Shared.Dtos.Items;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Services;

public interface IItemService
{
    Task<Result<PagedResult<ItemDto>>> SearchAsync(ItemSearchParameters p, CancellationToken ct = default);
    Task<Result<ItemDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<ItemDto>> CreateAsync(CreateItemRequest r, string userId, CancellationToken ct = default);
    Task<Result<ItemDto>> UpdateAsync(int id, UpdateItemRequest r, string userId, CancellationToken ct = default);
    Task<Result> DeleteAsync(int id, string userId, CancellationToken ct = default);
}

public class ItemService : IItemService
{
    private readonly ApplicationDbContext _db;

    public ItemService(ApplicationDbContext db) => _db = db;

    private static IQueryable<Item> WithNavs(IQueryable<Item> q) =>
        q.Include(i => i.Category)
         .Include(i => i.Location)
         .Include(i => i.ReportedBy)
         .Include(i => i.Images);

    public async Task<Result<PagedResult<ItemDto>>> SearchAsync(ItemSearchParameters p, CancellationToken ct = default)
    {
        var q = WithNavs(_db.Items);

        if (p.Type.HasValue) q = q.Where(i => i.Type == p.Type.Value);
        if (p.Status.HasValue) q = q.Where(i => i.Status == p.Status.Value);
        if (p.CategoryId.HasValue) q = q.Where(i => i.CategoryId == p.CategoryId.Value);
        if (p.LocationId.HasValue) q = q.Where(i => i.LocationId == p.LocationId.Value);
        if (p.FromDate.HasValue) q = q.Where(i => i.DateOccurred >= p.FromDate.Value);
        if (p.ToDate.HasValue) q = q.Where(i => i.DateOccurred <= p.ToDate.Value);

        if (!string.IsNullOrWhiteSpace(p.SearchTerm))
        {
            var term = p.SearchTerm.Trim();
            q = q.Where(i => i.Title.Contains(term) || i.Description.Contains(term));
        }

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(i => i.CreatedAt)
            .Skip((p.Page - 1) * p.PageSize)
            .Take(p.PageSize)
            .ToListAsync(ct);

        return Result<PagedResult<ItemDto>>.Ok(new PagedResult<ItemDto>
        {
            Items = items.Select(Map).ToList(),
            TotalCount = total,
            Page = p.Page,
            PageSize = p.PageSize
        });
    }

    public async Task<Result<ItemDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var item = await WithNavs(_db.Items).FirstOrDefaultAsync(i => i.Id == id, ct);
        return item is null
            ? Result<ItemDto>.Fail("Item not found", 404)
            : Result<ItemDto>.Ok(Map(item));
    }

    public async Task<Result<ItemDto>> CreateAsync(CreateItemRequest r, string userId, CancellationToken ct = default)
    {
        if (!await _db.Categories.AnyAsync(c => c.Id == r.CategoryId, ct))
            return Result<ItemDto>.Fail("Category not found", 404);
        if (!await _db.Locations.AnyAsync(l => l.Id == r.LocationId, ct))
            return Result<ItemDto>.Fail("Location not found", 404);

        var item = new Item
        {
            Title = r.Title,
            Description = r.Description,
            Type = r.Type,
            CategoryId = r.CategoryId,
            LocationId = r.LocationId,
            DateOccurred = r.DateOccurred,
            VerificationQuestion = r.VerificationQuestion,
            ContactInfo = r.ContactInfo,
            ReportedById = userId,
            Status = ItemStatus.Open
        };

        _db.Items.Add(item);
        await _db.SaveChangesAsync(ct);

        return Result<ItemDto>.Ok(Map(await WithNavs(_db.Items).FirstAsync(i => i.Id == item.Id, ct)));
    }

    public async Task<Result<ItemDto>> UpdateAsync(int id, UpdateItemRequest r, string userId, CancellationToken ct = default)
    {
        var item = await WithNavs(_db.Items).FirstOrDefaultAsync(i => i.Id == id, ct);
        if (item is null) return Result<ItemDto>.Fail("Item not found", 404);
        if (item.ReportedById != userId) return Result<ItemDto>.Fail("You can only edit your own reports", 403);

        if (!await _db.Categories.AnyAsync(c => c.Id == r.CategoryId, ct))
            return Result<ItemDto>.Fail("Category not found", 404);
        if (!await _db.Locations.AnyAsync(l => l.Id == r.LocationId, ct))
            return Result<ItemDto>.Fail("Location not found", 404);

        item.Title = r.Title;
        item.Description = r.Description;
        item.CategoryId = r.CategoryId;
        item.LocationId = r.LocationId;
        item.DateOccurred = r.DateOccurred;
        item.VerificationQuestion = r.VerificationQuestion;
        item.ContactInfo = r.ContactInfo;
        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result<ItemDto>.Ok(Map(item));
    }

    public async Task<Result> DeleteAsync(int id, string userId, CancellationToken ct = default)
    {
        var item = await _db.Items.FirstOrDefaultAsync(i => i.Id == id, ct);
        if (item is null) return Result.Fail("Item not found", 404);
        if (item.ReportedById != userId) return Result.Fail("You can only delete your own reports", 403);

        _db.Items.Remove(item);
        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }

    private static ItemDto Map(Item i) => new()
    {
        Id = i.Id,
        Title = i.Title,
        Description = i.Description,
        Type = i.Type,
        Status = i.Status,
        CategoryId = i.CategoryId,
        CategoryName = i.Category?.Name ?? string.Empty,
        LocationId = i.LocationId,
        LocationName = i.Location?.Name ?? string.Empty,
        ReportedById = i.ReportedById,
        ReportedByName = i.ReportedBy?.FullName ?? string.Empty,
        DateOccurred = i.DateOccurred,
        CreatedAt = i.CreatedAt,
        UpdatedAt = i.UpdatedAt,
        VerificationQuestion = i.VerificationQuestion,
        ContactInfo = i.ContactInfo,
        PrimaryImage = i.Images.FirstOrDefault(x => x.IsPrimary)?.FilePath ?? i.Images.FirstOrDefault()?.FilePath,
        ClaimCount = i.Claims.Count
    };
}
