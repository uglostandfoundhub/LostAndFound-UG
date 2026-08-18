namespace LostAndFound.Domain.Entities;

public class Location
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Building { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();
}
