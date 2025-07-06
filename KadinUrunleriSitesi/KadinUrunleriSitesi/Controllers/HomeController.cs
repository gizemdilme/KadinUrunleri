using KadinUrunleriSitesi.Models;
using KadinUrunleriSitesi.Models.ViewModels;
using KadinUrunleriSitesi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace KadinUrunleriSitesi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly KadinUrunPlatformuContext _context;
        private readonly FileUploadService _fileUploadService;
        
        public HomeController(ILogger<HomeController> logger, KadinUrunPlatformuContext context, FileUploadService fileUploadService)
        {
            _logger = logger;
            _context = context;
            _fileUploadService = fileUploadService;
        }
        
        [Authorize]
        public async Task<IActionResult> Index(string? arama, int page = 1)
        {
            int pageSize = 10;
            int skip = (page - 1) * pageSize;

            var query = _context.Urunlers
                .Include(u => u.EkleyenKullanici)
                .Include(u => u.Kategori)
                .Include(u => u.Puanlars)
                .Include(u => u.Yorumlars)
                    .ThenInclude(y => y.Kullanici)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(arama))
            {
                query = query.Where(u =>
                    u.UrunAdi.Contains(arama) ||
                    u.Aciklama.Contains(arama) ||
                    u.Kategori.KategoriAdi.Contains(arama));
            }

            var urunler = await query
                .OrderByDescending(u => u.EklenmeTarihi)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var feedViewModel = new FeedViewModel
            {
                CurrentPage = page,
                TotalPages = totalPages,
                Search = arama,
                Urunler = urunler.Select(u => new UrunFeedItem
                {
                    UrunId = u.UrunId,
                    UrunAdi = u.UrunAdi,
                    Aciklama = u.Aciklama,
                    ResimUrl = u.ResimUrl,
                    EklenmeTarihi = u.EklenmeTarihi ?? DateTime.Now,
                    EkleyenKullaniciAdi = u.EkleyenKullanici.AdSoyad,
                    EkleyenKullaniciProfilFoto = u.EkleyenKullanici.ProfilFotoUrl,
                    OrtalamaPuan = u.Puanlars.Any() ? u.Puanlars.Average(p => p.Puan) : 0,
                    PuanSayisi = u.Puanlars.Count,
                    YorumSayisi = u.Yorumlars.Count,
                    SonYorumlar = u.Yorumlars
                        .OrderByDescending(y => y.YorumTarihi)
                        .Take(3)
                        .Select(y => new YorumViewModel
                        {
                            YorumId = y.YorumId,
                            KullaniciAdi = y.Kullanici.AdSoyad,
                            Icerik = y.Icerik,
                            YorumFotoUrl = y.YorumFotoUrl,
                            YorumTarihi = y.YorumTarihi ?? DateTime.Now
                        }).ToList()
                }).ToList()
            };

            return View(feedViewModel);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Contact(string AdSoyad, string Email, string Mesaj)
        {
            TempData["Message"] = "Mesajınız başarıyla alındı.";
            return RedirectToAction("Contact");
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        [Authorize]
        public async Task<IActionResult> UrunDetay(int id)
        {
            var userId = User.FindFirstValue("KullaniciID");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int currentUserId))
            {
                return RedirectToAction("Giris", "Login");
            }

            var urun = await _context.Urunlers
                .Include(u => u.EkleyenKullanici)
                .Include(u => u.Kategori)
                .Include(u => u.Yorumlars)
                    .ThenInclude(y => y.Kullanici)
                .Include(u => u.Puanlars)
                .FirstOrDefaultAsync(u => u.UrunId == id);

            if (urun == null)
            {
                return NotFound();
            }

            // Calculate average rating
            double ortalamaPuan = 0;
            if (urun.Puanlars.Any())
            {
                ortalamaPuan = urun.Puanlars.Average(p => p.Puan);
            }

            // Get user's rating if exists
            int kullaniciPuani = 0;
            var existingPuan = urun.Puanlars.FirstOrDefault(p => p.KullaniciId == currentUserId);
            if (existingPuan != null)
            {
                kullaniciPuani = existingPuan.Puan;
            }

            var viewModel = new UrunDetayViewModel
            {
                Urun = urun,
                Yorumlar = urun.Yorumlars.OrderByDescending(y => y.YorumTarihi).ToList(),
                OrtalamaPuan = ortalamaPuan,
                PuanSayisi = urun.Puanlars.Count,
                KullaniciPuani = kullaniciPuani,
                YeniYorum = new YorumEkleViewModel { UrunId = urun.UrunId }
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YorumEkle(YorumEkleViewModel model)
        {
            var userId = User.FindFirstValue("KullaniciID");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int currentUserId))
            {
                return RedirectToAction("Giris", "Login");
            }

            // Verify that the product exists
            var urunExists = await _context.Urunlers.AnyAsync(u => u.UrunId == model.UrunId);
            if (!urunExists)
            {
                // Product doesn't exist, redirect to home page
                TempData["ErrorMessage"] = "Yorum eklemek istediğiniz ürün bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var yorum = new Yorumlar
                    {
                        UrunId = model.UrunId,
                        KullaniciId = currentUserId,
                        Icerik = model.Icerik,
                        YorumTarihi = DateTime.Now
                    };

                    // Handle image upload
                    if (model.YorumResim != null)
                    {
                        yorum.YorumFotoUrl = await _fileUploadService.UploadFileAsync(model.YorumResim, "yorum-resimleri");
                    }

                    _context.Yorumlars.Add(yorum);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Yorumunuz başarıyla eklendi.";
                    return RedirectToAction(nameof(UrunDetay), new { id = model.UrunId });
                }
                catch (Exception ex)
                {
                    // Log the exception
                    _logger.LogError(ex, "Yorum eklenirken hata oluştu.");
                    TempData["ErrorMessage"] = "Yorum eklenirken bir hata oluştu. Lütfen tekrar deneyiniz.";
                }
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction(nameof(UrunDetay), new { id = model.UrunId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PuanVer(int urunId, int puan)
        {
            var userId = User.FindFirstValue("KullaniciID");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int currentUserId))
            {
                return RedirectToAction("Giris", "Login");
            }

            if (puan < 1 || puan > 5)
            {
                TempData["ErrorMessage"] = "Geçersiz puan değeri. Puan 1-5 arasında olmalıdır.";
                return RedirectToAction(nameof(UrunDetay), new { id = urunId });
            }

            // Check if user already rated this product
            var existingPuan = await _context.Puanlars
                .FirstOrDefaultAsync(p => p.UrunId == urunId && p.KullaniciId == currentUserId);

            if (existingPuan != null)
            {
                // Update existing rating
                existingPuan.Puan = puan;
                _context.Update(existingPuan);
            }
            else
            {
                // Add new rating
                var yeniPuan = new Puanlar
                {
                    UrunId = urunId,
                    KullaniciId = currentUserId,
                    Puan = puan
                };
                _context.Add(yeniPuan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UrunDetay), new { id = urunId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YorumSil(int yorumId, int urunId)
        {
            var userId = User.FindFirstValue("KullaniciID");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int currentUserId))
            {
                return RedirectToAction("Giris", "Login");
            }

            var yorum = await _context.Yorumlars.FindAsync(yorumId);

            if (yorum == null)
            {
                TempData["ErrorMessage"] = "Yorum bulunamadı.";
                return RedirectToAction(nameof(UrunDetay), new { id = urunId });
            }

            // Check if the current user is the owner of the comment
            if (yorum.KullaniciId != currentUserId)
            {
                TempData["ErrorMessage"] = "Bu yorumu silme yetkiniz yok.";
                return RedirectToAction(nameof(UrunDetay), new { id = urunId });
            }

            try
            {
                // Delete the comment image if it exists
                if (!string.IsNullOrEmpty(yorum.YorumFotoUrl))
                {
                    // Get the physical path of the image
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", yorum.YorumFotoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Yorumlars.Remove(yorum);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Yorum başarıyla silindi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yorum silinirken hata oluştu.");
                TempData["ErrorMessage"] = "Yorum silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(UrunDetay), new { id = urunId });
        }

        [Authorize]
        public async Task<IActionResult> Gecmisim()
        {
            // Get current user ID from claims
            var userId = User.FindFirstValue("KullaniciID");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int currentUserId))
            {
                return RedirectToAction("Giris", "Login");
            }

            // Get user's comments with related products
            var userComments = await _context.Yorumlars
                .Include(y => y.Urun)
                    .ThenInclude(u => u.Kategori)
                .Include(y => y.Kullanici)
                .Where(y => y.KullaniciId == currentUserId)
                .OrderByDescending(y => y.YorumTarihi)
                .ToListAsync();

            // Get user's ratings with related products
            var userRatings = await _context.Puanlars
                .Include(p => p.Urun)
                    .ThenInclude(u => u.Kategori)
                .Include(p => p.Kullanici)
                .Where(p => p.KullaniciId == currentUserId)
                .OrderByDescending(p => p.Urun.EklenmeTarihi)
                .ToListAsync();

            // Create view model
            var viewModel = new GecmisimViewModel
            {
                Yorumlar = userComments,
                Puanlar = userRatings
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
