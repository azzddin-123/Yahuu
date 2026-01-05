using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProfileController : Controller
{
    private readonly AppDbContext _context;

    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
        if (userId == 0)
            return RedirectToAction("Login", "Account");

        var user = await _context.Users
            .Include(u => u.Posts)
                .ThenInclude(p => p.Likes)
            .Include(u => u.Posts)
                .ThenInclude(p => p.Comments)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return View(user);
    }
    // ===================== EDIT NAME =====================
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
        if (userId == 0)
            return RedirectToAction("Login", "Account");

        var user = await _context.Users.FindAsync(userId);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string nom)
    {
        int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
        if (userId == 0)
            return RedirectToAction("Login", "Account");

        var user = await _context.Users.FindAsync(userId);
        user.Nom = nom;

        await _context.SaveChangesAsync();

        // mettre à jour la session
        HttpContext.Session.SetString("UserName", nom);

        return RedirectToAction("Index");
    }
}