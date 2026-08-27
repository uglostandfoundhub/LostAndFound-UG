namespace LostAndFound.Shared.Dtos.Notifications;

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? RelatedItemId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
