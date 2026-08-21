namespace LostAndFound.Domain.Entities;

public class ItemImage
{
    public int Id { get; set; }

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public string FilePath { get; set; } = string.Empty;

    public string? FileName { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
