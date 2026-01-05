using System.Collections.Generic;

public class User
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } // Admin / User
    public string? Photo { get; set; }
    public bool IsBlocked { get; set; }

    // ✅ AJOUT OBLIGATOIRE
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}
