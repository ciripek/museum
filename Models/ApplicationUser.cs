using Microsoft.AspNetCore.Identity;

namespace museum.Models;

public class ApplicationUser : IdentityUser, IBaseEntity
{
    public bool IsAdmin { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}