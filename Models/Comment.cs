using System.ComponentModel.DataAnnotations.Schema;

namespace museum.Models;

public class Comment : IBaseEntity
{
    public int Id { get; set; }

    [Column(TypeName = "text")] public required string Text { get; set; }

    public required Item Item { get; set; }
    public required ApplicationUser ApplicationUser { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}