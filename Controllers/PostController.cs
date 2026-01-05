using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class PostController : Controller
{
    private readonly AppDbContext _context;

    public PostController(AppDbContext context)
    {
        _context = context;
    }

    // ================= CREATE POST PAGE =================
    [HttpGet]
    public IActionResult Create()
    {
        // Sécurité : utilisateur connecté ?
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToAction("Login", "Account");

        return View();
    }

    // ================= CREATE POST ACTION =================
    [HttpPost]
    public async Task<IActionResult> Create(string content, IFormFile image)
    {
        int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

        if (userId == 0)
            return RedirectToAction("Login", "Account");

        string imageName = null;

        if (image != null)
        {
            imageName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", imageName);

            using var stream = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(stream);
        }

        var post = new Post
        {
            Content = content,
            Image = imageName,
            CreatedAt = DateTime.Now,
            UserId = userId
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
    // ===================== LIKE =====================
    public async Task<IActionResult> Like(int id)
    {
        int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
        if (userId == 0)
            return RedirectToAction("Login", "Account");

        var existLike = await _context.Likes
            .FirstOrDefaultAsync(l => l.PostId == id && l.UserId == userId);

        if (existLike == null)
        {
            _context.Likes.Add(new Like
            {
                PostId = id,
                UserId = userId
            });
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Home");
    }

    // ===================== ADD COMMENT =====================
    [HttpPost]
    public async Task<IActionResult> AddComment(int postId, string content)
    {
        int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
        if (userId == 0)
            return RedirectToAction("Login", "Account");

        var comment = new Comment
        {
            Content = content,
            CreatedAt = DateTime.Now,
            PostId = postId,
            UserId = userId
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
}

