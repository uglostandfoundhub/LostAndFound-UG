using LostAndFound.Domain.Enums;

namespace LostAndFound.Domain.Entities;

public class Item
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ItemType Type { get; set; }

    public ItemStatus Status { get; set; } = ItemStatus.Open;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public string ReportedById { get; set; } = string.Empty;
    public ApplicationUser ReportedBy { get; set; } = null!;

    public DateTime DateOccurred { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public string? VerificationQuestion { get; set; }

    public string? ContactInfo { get; set; }

    public ICollection<ItemImage> Images { get; set; } = new List<ItemImage>();
    public ICollection<ItemClaim> Claims { get; set; } = new List<ItemClaim>();
}
