namespace LostAndFound.Domain.Entities;

public class ItemMatch
{
    public int Id { get; set; }

    public int LostItemId { get; set; }
    public Item LostItem { get; set; } = null!;

    public int FoundItemId { get; set; }
    public Item FoundItem { get; set; } = null!;

    public double Score { get; set; }

    public bool IsConfirmed { get; set; }

    public bool IsDismissed { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
