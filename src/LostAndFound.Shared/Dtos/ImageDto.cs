namespace LostAndFound.Shared.Dtos;

public class ImageDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime UploadedAt { get; set; }
}
