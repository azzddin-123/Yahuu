using System;
using System.Collections.Generic;

public class Post
{
    public int Id { get; set; }
    public string? Content { get; set; }
    public string? Image { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int UserId { get; set; }
    public User User { get; set; }

    // ✅ AJOUT OBLIGATOIRE
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}
