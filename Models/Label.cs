namespace museum.Models;

public class Label : IBaseEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public bool Display { get; set; }
    public required string Color { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}