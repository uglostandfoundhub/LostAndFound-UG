using LostAndFound.Domain.Enums;

namespace LostAndFound.Shared.Dtos.Items;

public class ItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ItemType Type { get; set; }
    public ItemStatus Status { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string ReportedById { get; set; } = string.Empty;
    public string ReportedByName { get; set; } = string.Empty;
    public DateTime DateOccurred { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? VerificationQuestion { get; set; }
    public string? ContactInfo { get; set; }
    public string? PrimaryImage { get; set; }
    public int ClaimCount { get; set; }
}

public class CreateItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ItemType Type { get; set; }
    public int CategoryId { get; set; }
    public int LocationId { get; set; }
    public DateTime DateOccurred { get; set; }
    public string? VerificationQuestion { get; set; }
    public string? ContactInfo { get; set; }
}

public class UpdateItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int LocationId { get; set; }
    public DateTime DateOccurred { get; set; }
    public string? VerificationQuestion { get; set; }
    public string? ContactInfo { get; set; }
}

public class ItemSearchParameters
{
    public ItemType? Type { get; set; }
    public ItemStatus? Status { get; set; }
    public int? CategoryId { get; set; }
    public int? LocationId { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
