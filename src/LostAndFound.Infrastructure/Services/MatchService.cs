using LostAndFound.Domain.Entities;
using LostAndFound.Domain.Enums;
using LostAndFound.Infrastructure.Data;
using LostAndFound.Shared.Dtos;
using LostAndFound.Shared.Dtos.Matches;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Services;

public interface IMatchService
{
    Task<Result<IReadOnlyList<MatchDto>>> GetForUserAsync(string userId, CancellationToken ct = default);
    Task<Result<IReadOnlyList<MatchDto>>> GenerateForItemAsync(int itemId, string userId, CancellationToken ct = default);
    Task<Result> DismissAsync(int matchId, string userId, CancellationToken ct = default);
}

public class MatchService : IMatchService
{
    private readonly ApplicationDbContext _db;

    public MatchService(ApplicationDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<MatchDto>>> GetForUserAsync(string userId, CancellationToken ct = default)
    {
        var matches = await _db.ItemMatches
            .Include(m => m.LostItem)
            .Include(m => m.FoundItem)
            .Where(m => m.LostItem.ReportedById == userId || m.FoundItem.ReportedById == userId)
            .OrderByDescending(m => m.Score)
            .ToListAsync(ct);

        return Result<IReadOnlyList<MatchDto>>.Ok(matches.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<MatchDto>>> GenerateForItemAsync(int itemId, string userId, CancellationToken ct = default)
    {
        var item = await _db.Items
            .Include(i => i.Images)
            .FirstOrDefaultAsync(i => i.Id == itemId, ct);
        if (item is null) return Result<IReadOnlyList<MatchDto>>.Fail("Item not found", 404);
        if (item.ReportedById != userId) return Result<IReadOnlyList<MatchDto>>.Fail("You can only generate matches for your own reports", 403);

        var opposite = item.Type == ItemType.Lost ? ItemType.Found : ItemType.Lost;
        var candidates = await _db.Items
            .Where(i => i.Type == opposite && i.Status == ItemStatus.Open && i.Id != item.Id)
            .ToListAsync(ct);

        var created = new List<MatchDto>();
        foreach (var candidate in candidates)
        {
            var score = Score(item, candidate);
            if (score < 0.4) continue;

            var exists = await _db.ItemMatches.AnyAsync(
                m => m.LostItemId == (item.Type == ItemType.Lost ? item.Id : candidate.Id) &&
                     m.FoundItemId == (item.Type == ItemType.Lost ? candidate.Id : item.Id), ct);
            if (exists) continue;

            var (lostId, foundId) = item.Type == ItemType.Lost ? (item.Id, candidate.Id) : (candidate.Id, item.Id);
            var match = new ItemMatch
            {
                LostItemId = lostId,
                FoundItemId = foundId,
                Score = score
            };
            _db.ItemMatches.Add(match);
            await _db.SaveChangesAsync(ct);

            var otherPartyId = item.Type == ItemType.Lost ? candidate.ReportedById : item.ReportedById;
            _db.Notifications.Add(new Notification
            {
                UserId = otherPartyId,
                Title = "Possible match found",
                Message = $"We found a possible match for \"{candidate.Title}\" related to your report \"{item.Title}\".",
                RelatedItemId = item.Id
            });

            created.Add(Map(await _db.ItemMatches
                .Include(m => m.LostItem).Include(m => m.FoundItem)
                .FirstAsync(m => m.Id == match.Id, ct)));
        }

        return Result<IReadOnlyList<MatchDto>>.Ok(created);
    }

    public async Task<Result> DismissAsync(int matchId, string userId, CancellationToken ct = default)
    {
        var match = await _db.ItemMatches
            .Include(m => m.LostItem).Include(m => m.FoundItem)
            .FirstOrDefaultAsync(m => m.Id == matchId, ct);
        if (match is null) return Result.Fail("Match not found", 404);
        if (match.LostItem.ReportedById != userId && match.FoundItem.ReportedById != userId)
            return Result.Fail("Not authorized", 403);

        match.IsDismissed = true;
        await _db.SaveChangesAsync(ct);
        return Result.Ok();
    }

    private static double Score(Item a, Item b)
    {
        double score = 0;
        if (a.CategoryId == b.CategoryId) score += 0.4;
        if (a.LocationId == b.LocationId) score += 0.3;

        var aWords = Words(a.Title);
        var bWords = Words(b.Title);
        if (aWords.Count > 0 && bWords.Count > 0)
        {
            var overlap = aWords.Intersect(bWords).Count();
            score += 0.3 * (overlap / (double)Math.Max(aWords.Count, bWords.Count));
        }

        return Math.Min(score, 1);
    }

    private static HashSet<string> Words(string text) =>
        new((text ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(w => w.Length > 2), StringComparer.OrdinalIgnoreCase);

    private static MatchDto Map(ItemMatch m) => new()
    {
        Id = m.Id,
        LostItemId = m.LostItemId,
        LostItemTitle = m.LostItem?.Title ?? string.Empty,
        FoundItemId = m.FoundItemId,
        FoundItemTitle = m.FoundItem?.Title ?? string.Empty,
        Score = m.Score,
        IsConfirmed = m.IsConfirmed,
        IsDismissed = m.IsDismissed,
        CreatedAt = m.CreatedAt
    };
}
