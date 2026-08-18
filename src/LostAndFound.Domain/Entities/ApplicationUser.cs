using Microsoft.AspNetCore.Identity;

namespace LostAndFound.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public string? StudentId { get; set; }

    public string? Department { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public ICollection<Item> ReportedItems { get; set; } = new List<Item>();
    public ICollection<ItemClaim> SubmittedClaims { get; set; } = new List<ItemClaim>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
