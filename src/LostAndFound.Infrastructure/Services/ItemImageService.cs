using LostAndFound.Domain.Entities;
using LostAndFound.Infrastructure.Data;
using LostAndFound.Shared.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Services;

public interface IItemImageService
{
    Task<Result<ImageDto>> UploadAsync(int itemId, string fileName, Stream content, bool isPrimary, CancellationToken ct = default);
    Task<Result<IReadOnlyList<ImageDto>>> GetForItemAsync(int itemId, CancellationToken ct = default);
    Task<Result> DeleteAsync(int itemId, int imageId, CancellationToken ct = default);
}

public class ItemImageService : IItemImageService
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp"
    };

    public ItemImageService(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<Result<ImageDto>> UploadAsync(int itemId, string fileName, Stream content, bool isPrimary, CancellationToken ct = default)
    {
        var item = await _db.Items.FirstOrDefaultAsync(i => i.Id == itemId, ct);
        if (item is null) return Result<ImageDto>.Fail("Item not found", 404);

        var ext = Path.GetExtension(fileName);
        if (!AllowedExtensions.Contains(ext)) return Result<ImageDto>.Fail("Unsupported image type.", 400);

        var uploadsDir = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsDir);
        var storedName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(uploadsDir, storedName);

        await using (var fs = File.Create(fullPath))
        {
            await content.CopyToAsync(fs, ct);
        }

        var image = new ItemImage
        {
            ItemId = itemId,
            FilePath = $"/uploads/{storedName}",
            FileName = fileName,
            IsPrimary = isPrimary || !await _db.ItemImages.AnyAsync(i => i.ItemId == itemId, ct)
        };

        _db.ItemImages.Add(image);
        await _db.SaveChangesAsync(ct);

        return Result<ImageDto>.Ok(Map(image));
    }

    public async Task<Result<IReadOnlyList<ImageDto>>> GetForItemAsync(int itemId, CancellationToken ct = default)
    {
        var images = await _db.ItemImages
            .Where(i => i.ItemId == itemId)
            .OrderByDescending(i => i.IsPrimary)
            .ToListAsync(ct);
        return Result<IReadOnlyList<ImageDto>>.Ok(images.Select(Map).ToList());
    }

    public async Task<Result> DeleteAsync(int itemId, int imageId, CancellationToken ct = default)
    {
        var image = await _db.ItemImages.FirstOrDefaultAsync(i => i.Id == imageId && i.ItemId == itemId, ct);
        if (image is null) return Result.Fail("Image not found", 404);

        var relative = image.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_env.ContentRootPath, relative);
        if (File.Exists(fullPath)) File.Delete(fullPath);

        _db.ItemImages.Remove(image);
        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }

    private static ImageDto Map(ItemImage i) => new()
    {
        Id = i.Id,
        ItemId = i.ItemId,
        FilePath = i.FilePath,
        FileName = i.FileName,
        IsPrimary = i.IsPrimary,
        UploadedAt = i.UploadedAt
    };
}
