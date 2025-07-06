using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using KadinUrunleriSitesi.Models;
using KadinUrunleriSitesi.Services;

namespace KadinUrunleriSitesi.Controllers
{
    public class ProfilController : Controller
    {
        private readonly KadinUrunPlatformuContext _context;
        private readonly FileUploadService _fileService;

        public ProfilController(KadinUrunPlatformuContext context, FileUploadService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var kullaniciIdStr = User.FindFirstValue("KullaniciID");
            if (!int.TryParse(kullaniciIdStr, out int kullaniciId))
                return RedirectToAction("Giris", "Login");

            var kullanici = await _context.Kullanicilars
                .FirstOrDefaultAsync(k => k.KullaniciId == kullaniciId);

            if (kullanici == null)
                return NotFound();

            // Kullanıcının ürünleri
            var urunler = await _context.Urunlers
                .Where(u => u.EkleyenKullaniciId == kullaniciId)
                .OrderByDescending(u => u.EklenmeTarihi)
                .ToListAsync();
            ViewBag.Urunler = urunler;

            // Kullanıcının arkadaşları
            var arkadaslar = await _context.Arkadasliklar
                .Where(a => a.Durum == "Kabul" &&
                           (a.GonderenId == kullaniciId || a.AlanId == kullaniciId))
                .Select(a => a.GonderenId == kullaniciId ? a.Alan : a.Gonderen)
                .ToListAsync();
            ViewBag.Arkadaslar = arkadaslar;

            return View(kullanici);
        }

        public IActionResult Search(string q)
        {
            var kisiler = string.IsNullOrWhiteSpace(q)
                ? new List<Kullanicilar>()
                : _context.Kullanicilars
                    .Where(k => k.AdSoyad.Contains(q) || k.Email.Contains(q))
                    .ToList();

            return View(kisiler);
        }

        public IActionResult Detay(int id)
        {
            var kullanici = _context.Kullanicilars.FirstOrDefault(k => k.KullaniciId == id);
            if (kullanici == null)
                return NotFound();

            return View(kullanici);
        }

        public async Task<IActionResult> Bildirimler()
        {
            var kullaniciIdStr = User.FindFirstValue("KullaniciID");
            if (!int.TryParse(kullaniciIdStr, out int kullaniciId))
                return RedirectToAction("Giris", "Login");

            var istekler = await _context.Arkadasliklar
                .Include(a => a.Gonderen)
                .Where(a => a.AlanId == kullaniciId && a.Durum == "Beklemede")
                .ToListAsync();

            return View(istekler);
        }

        public async Task<IActionResult> Arkadaslar()
        {
            var kullaniciIdStr = User.FindFirstValue("KullaniciID");
            if (!int.TryParse(kullaniciIdStr, out int kullaniciId))
                return RedirectToAction("Giris", "Login");

            var arkadaslar = await _context.Arkadasliklar
                .Where(a => a.Durum == "Kabul" &&
                           (a.GonderenId == kullaniciId || a.AlanId == kullaniciId))
                .Select(a => a.GonderenId == kullaniciId ? a.AlanId : a.GonderenId)
                .Distinct()
                .ToListAsync();

            var arkadasKullanicilar = await _context.Kullanicilars
                .Where(k => arkadaslar.Contains(k.KullaniciId))
                .Select(k => new ArkadaslarViewModel
                {
                    KullaniciId = k.KullaniciId,
                    AdSoyad = k.AdSoyad,
                    ProfilFotoUrl = k.ProfilFotoUrl
                })
                .ToListAsync();

            return View(arkadasKullanicilar);
        }

        [HttpGet]
        public IActionResult Duzenle()
        {
            var kullaniciIdStr = User.FindFirstValue("KullaniciID");
            if (!int.TryParse(kullaniciIdStr, out int kullaniciId))
                return RedirectToAction("Giris", "Login");

            var kullanici = _context.Kullanicilars.FirstOrDefault(k => k.KullaniciId == kullaniciId);
            if (kullanici == null)
                return NotFound();

            return View(kullanici);
        }

        [HttpPost]
        public async Task<IActionResult> Duzenle(Kullanicilar model, IFormFile? yeniFoto)
        {
            if (!ModelState.IsValid)
                return View(model);

            var mevcut = await _context.Kullanicilars.FindAsync(model.KullaniciId);
            if (mevcut == null)
                return NotFound();

            mevcut.AdSoyad = model.AdSoyad;
            mevcut.Email = model.Email;
            mevcut.Sifre = model.Sifre;

            if (yeniFoto != null)
            {
                var url = await _fileService.UploadFileAsync(yeniFoto, "images/users");
                mevcut.ProfilFotoUrl = url;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
