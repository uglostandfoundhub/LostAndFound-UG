using LostAndFound.Domain.Entities;
using LostAndFound.Domain.Enums;
using LostAndFound.Infrastructure.Data;
using LostAndFound.Shared.Dtos;
using LostAndFound.Shared.Dtos.Claims;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Services;

public interface IClaimService
{
    Task<Result<ClaimDto>> CreateAsync(int itemId, CreateClaimRequest r, string userId, CancellationToken ct = default);
    Task<Result<IReadOnlyList<ClaimDto>>> GetForItemAsync(int itemId, CancellationToken ct = default);
    Task<Result<IReadOnlyList<ClaimDto>>> GetMineAsync(string userId, CancellationToken ct = default);
    Task<Result<ClaimDto>> ReviewAsync(int claimId, ReviewClaimRequest r, string reviewerId, CancellationToken ct = default);
}

public class ClaimService : IClaimService
{
    private readonly ApplicationDbContext _db;

    public ClaimService(ApplicationDbContext db) => _db = db;

    private static IQueryable<ItemClaim> WithNavs(IQueryable<ItemClaim> q) =>
        q.Include(c => c.Item).Include(c => c.Claimant);

    public async Task<Result<ClaimDto>> CreateAsync(int itemId, CreateClaimRequest r, string userId, CancellationToken ct = default)
    {
        var item = await _db.Items.FirstOrDefaultAsync(i => i.Id == itemId, ct);
        if (item is null) return Result<ClaimDto>.Fail("Item not found", 404);
        if (item.Type != ItemType.Found) return Result<ClaimDto>.Fail("Claims can only be made on found items", 400);
        if (item.Status is ItemStatus.Claimed or ItemStatus.Returned or ItemStatus.Archived)
            return Result<ClaimDto>.Fail("This item is no longer open for claims", 400);

        var claim = new ItemClaim
        {
            ItemId = itemId,
            ClaimantId = userId,
            VerificationAnswer = r.VerificationAnswer,
            SupportingDetails = r.SupportingDetails,
            Status = ClaimStatus.Pending
        };

        if (item.Status == ItemStatus.Open) item.Status = ItemStatus.UnderReview;

        _db.ItemClaims.Add(claim);
        await _db.SaveChangesAsync(ct);

        return Result<ClaimDto>.Ok(Map(await WithNavs(_db.ItemClaims).FirstAsync(c => c.Id == claim.Id, ct)));
    }

    public async Task<Result<IReadOnlyList<ClaimDto>>> GetForItemAsync(int itemId, CancellationToken ct = default)
    {
        var claims = await WithNavs(_db.ItemClaims)
            .Where(c => c.ItemId == itemId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);
        return Result<IReadOnlyList<ClaimDto>>.Ok(claims.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<ClaimDto>>> GetMineAsync(string userId, CancellationToken ct = default)
    {
        var claims = await WithNavs(_db.ItemClaims)
            .Where(c => c.ClaimantId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);
        return Result<IReadOnlyList<ClaimDto>>.Ok(claims.Select(Map).ToList());
    }

    public async Task<Result<ClaimDto>> ReviewAsync(int claimId, ReviewClaimRequest r, string reviewerId, CancellationToken ct = default)
    {
        var claim = await WithNavs(_db.ItemClaims).FirstOrDefaultAsync(c => c.Id == claimId, ct);
        if (claim is null) return Result<ClaimDto>.Fail("Claim not found", 404);
        if (claim.Status != ClaimStatus.Pending) return Result<ClaimDto>.Fail("Claim has already been reviewed", 400);

        claim.Status = r.Status;
        claim.ReviewNotes = r.ReviewNotes;
        claim.ReviewedById = reviewerId;
        claim.ReviewedAt = DateTime.UtcNow;

        if (r.Status == ClaimStatus.Approved)
        {
            claim.Item.Status = ItemStatus.Claimed;
            _db.Notifications.Add(new Notification
            {
                UserId = claim.ClaimantId,
                Title = "Claim approved",
                Message = $"Your claim on \"{claim.Item.Title}\" was approved. Arrange collection with the lost & found office.",
                RelatedItemId = claim.ItemId
            });
        }
        else
        {
            var stillPending = await _db.ItemClaims.AnyAsync(c => c.ItemId == claim.ItemId && c.Id != claim.Id && c.Status == ClaimStatus.Pending, ct);
            if (!stillPending) claim.Item.Status = ItemStatus.Open;

            _db.Notifications.Add(new Notification
            {
                UserId = claim.ClaimantId,
                Title = "Claim rejected",
                Message = $"Your claim on \"{claim.Item.Title}\" was not approved. {r.ReviewNotes ?? string.Empty}".Trim(),
                RelatedItemId = claim.ItemId
            });
        }

        await _db.SaveChangesAsync(ct);
        return Result<ClaimDto>.Ok(Map(await WithNavs(_db.ItemClaims).FirstAsync(c => c.Id == claim.Id, ct)));
    }

    private static ClaimDto Map(ItemClaim c) => new()
    {
        Id = c.Id,
        ItemId = c.ItemId,
        ItemTitle = c.Item?.Title ?? string.Empty,
        ClaimantId = c.ClaimantId,
        ClaimantName = c.Claimant?.FullName ?? string.Empty,
        VerificationAnswer = c.VerificationAnswer,
        SupportingDetails = c.SupportingDetails,
        Status = c.Status,
        ReviewedByName = c.ReviewedBy?.FullName,
        ReviewNotes = c.ReviewNotes,
        CreatedAt = c.CreatedAt,
        ReviewedAt = c.ReviewedAt
    };
}
