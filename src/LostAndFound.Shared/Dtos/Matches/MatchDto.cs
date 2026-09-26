namespace LostAndFound.Shared.Dtos.Matches;

public class MatchDto
{
    public int Id { get; set; }
    public int LostItemId { get; set; }
    public string LostItemTitle { get; set; } = string.Empty;
    public int FoundItemId { get; set; }
    public string FoundItemTitle { get; set; } = string.Empty;
    public double Score { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsDismissed { get; set; }
    public DateTime CreatedAt { get; set; }
}
