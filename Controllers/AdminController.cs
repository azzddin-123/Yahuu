using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard(string section = "users")
    {
        // 🔐 Sécurité Admin
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToAction("Login", "Account");

        // 📊 Stats globales
        ViewBag.UsersCount = await _context.Users.CountAsync(u => u.Role != "Admin");
        ViewBag.PostsCount = await _context.Posts.CountAsync();
        ViewBag.CommentsCount = await _context.Comments.CountAsync();
        ViewBag.BlockedUsersCount = await _context.Users.CountAsync(u => u.IsBlocked);

        ViewBag.AdminName = HttpContext.Session.GetString("UserName");
        ViewBag.Section = section;

        // 🔄 Données dynamiques selon la section
        if (section == "posts")
        {
            ViewBag.Posts = await _context.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
        else if (section == "comments")
        {
            ViewBag.Comments = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // 👥 Toujours charger les users
        var users = await _context.Users
            .Where(u => u.Role != "Admin")
            .ToListAsync();

        return View(users);
    }

    public async Task<IActionResult> Block(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsBlocked = true;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Dashboard", new { section = "users" });
    }

    public async Task<IActionResult> Unblock(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsBlocked = false;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Dashboard", new { section = "users" });
    }
    public async Task<IActionResult> DeletePost(int id)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Dashboard", new { section = "posts" });
    }
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _context.Comments.FindAsync(id);

        if (comment != null)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Dashboard", new { section = "comments" });
    }

}
