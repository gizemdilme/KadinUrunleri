using KadinUrunleriSitesi.Models;
using KadinUrunleriSitesi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class LoginController : Controller
{
    private readonly KadinUrunPlatformuContext _context;
    private readonly FileUploadService _fileUploadService;

    public LoginController(KadinUrunPlatformuContext context , FileUploadService fileUploadService)
    {
        _context = context;
        _fileUploadService = fileUploadService;
    }

    [HttpGet]
    public IActionResult Giris()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Giris(string email, string sifre)
    {
        var kullanici = _context.Kullanicilars
            .FirstOrDefault(k => k.Email == email && k.Sifre == sifre);

        if (kullanici == null)
        {
            ViewBag.Hata = "Geçersiz e-posta veya şifre";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, kullanici.AdSoyad),
            new Claim(ClaimTypes.Email, kullanici.Email),
            new Claim(ClaimTypes.Role, (kullanici.IsAdmin ?? false) ? "Admin" : "User"),

            new Claim("KullaniciID", kullanici.KullaniciId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("MyCookieAuth", principal);
        HttpContext.Session.SetString("kullaniciEmail", kullanici.Email);
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Cikis()
    {
        await HttpContext.SignOutAsync("MyCookieAuth");
        return RedirectToAction("Giris");
    }

    [HttpGet]
    public IActionResult Kayit()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Kayit(Kullanicilar kullanici, IFormFile? ProfilFoto)
    {
        if (!ModelState.IsValid)
            return View(kullanici);

        kullanici.KayitTarihi = DateTime.Now;
        kullanici.IsAdmin = false;

        // Handle file upload
        if (ProfilFoto != null && ProfilFoto.Length > 0)
        {
            kullanici.ProfilFotoUrl = await _fileUploadService.UploadFileAsync(ProfilFoto, "images/users");
        }

        _context.Kullanicilars.Add(kullanici);

        try
        {
            await _context.SaveChangesAsync();
            return RedirectToAction("Giris");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Kayıt sırasında bir hata oluştu: " + ex.Message);
            return View(kullanici);
        }
    }

}
