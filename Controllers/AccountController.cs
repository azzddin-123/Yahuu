using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yahuu.Models;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    // ===================== LOGIN PAGE =====================
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // ===================== REGISTER =====================
    [HttpPost]
    public async Task<IActionResult> Register(string nom, string email, string password)
    {
        // Vérifier si email existe déjà
        var existUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (existUser != null)
        {
            ViewBag.Error = "Email déjà utilisé";
            return View("Login");
        }

        var user = new User
        {
            Nom = nom,
            Email = email,
            PasswordHash = password, // ⚠️ Hash plus tard
            Role = "User",
            IsBlocked = false
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(); // 🔥 ENREGISTREMENT DB

        // 🔁 Retour à la page Login après inscription
        return RedirectToAction("Login");
    }

    // ===================== LOGIN =====================
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);

        if (user == null)
        {
            ViewBag.Error = "Email ou mot de passe incorrect";
            return View();
        }

        if (user.IsBlocked)
        {
            ViewBag.Error = "Votre compte est temporairement bloqué";
            return View();
        }

        // Session
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserRole", user.Role);
        HttpContext.Session.SetString("UserName", user.Nom);

        // Redirection selon rôle
        if (user.Role == "Admin")
            return RedirectToAction("Dashboard", "Admin");

        return RedirectToAction("Index", "Home");
    }

    // ===================== LOGOUT =====================
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
