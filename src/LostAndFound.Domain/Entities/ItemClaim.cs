using LostAndFound.Domain.Enums;

namespace LostAndFound.Domain.Entities;

public class ItemClaim
{
    public int Id { get; set; }

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public string ClaimantId { get; set; } = string.Empty;
    public ApplicationUser Claimant { get; set; } = null!;

    public string VerificationAnswer { get; set; } = string.Empty;

    public string? SupportingDetails { get; set; }

    public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

    public string? ReviewedById { get; set; }
    public ApplicationUser? ReviewedBy { get; set; }

    public string? ReviewNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }
}
