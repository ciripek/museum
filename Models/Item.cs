using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace museum.Models;

public class Item : IBaseEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    [Column(TypeName = "text")] public required string Description { get; set; }

    [DataType(DataType.Date)] public DateTime Obtained { get; set; }

    [DataType(DataType.Upload)] public string? Image { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Label> Labels { get; set; } = new List<Label>();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}