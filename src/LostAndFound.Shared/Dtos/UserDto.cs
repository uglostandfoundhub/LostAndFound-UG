namespace LostAndFound.Shared.Dtos;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? StudentId { get; set; }
    public string? Department { get; set; }
    public bool IsActive { get; set; }
    public IList<string> Roles { get; set; } = Array.Empty<string>();
}
