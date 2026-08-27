using LostAndFound.Domain.Enums;

namespace LostAndFound.Shared.Dtos.Claims;

public class ClaimDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemTitle { get; set; } = string.Empty;
    public string ClaimantId { get; set; } = string.Empty;
    public string ClaimantName { get; set; } = string.Empty;
    public string VerificationAnswer { get; set; } = string.Empty;
    public string? SupportingDetails { get; set; }
    public ClaimStatus Status { get; set; }
    public string? ReviewedByName { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

public class CreateClaimRequest
{
    public string VerificationAnswer { get; set; } = string.Empty;
    public string? SupportingDetails { get; set; }
}

public class ReviewClaimRequest
{
    public ClaimStatus Status { get; set; }
    public string? ReviewNotes { get; set; }
}
