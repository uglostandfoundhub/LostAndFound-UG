using LostAndFound.Domain.Entities;
using LostAndFound.Infrastructure.Data;
using LostAndFound.Shared.Dtos;
using LostAndFound.Shared.Dtos.Notifications;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Services;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetMineAsync(string userId, CancellationToken ct = default);
    Task<Result> MarkReadAsync(int id, string userId, CancellationToken ct = default);
}

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _db;

    public NotificationService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<NotificationDto>> GetMineAsync(string userId, CancellationToken ct = default) =>
        (await _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct))
        .Select(Map).ToList();

    public async Task<Result> MarkReadAsync(int id, string userId, CancellationToken ct = default)
    {
        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);
        if (n is null) return Result.Fail("Notification not found", 404);

        n.IsRead = true;
        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }

    private static NotificationDto Map(Notification n) => new()
    {
        Id = n.Id,
        Title = n.Title,
        Message = n.Message,
        RelatedItemId = n.RelatedItemId,
        IsRead = n.IsRead,
        CreatedAt = n.CreatedAt
    };
}
