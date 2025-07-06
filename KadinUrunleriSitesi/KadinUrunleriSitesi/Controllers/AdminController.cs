using KadinUrunleriSitesi.Models;
using KadinUrunleriSitesi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace KadinUrunleriSitesi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly KadinUrunPlatformuContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(KadinUrunPlatformuContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardViewModel = new AdminDashboardViewModel
            {
                KullaniciSayisi = await _context.Kullanicilars.CountAsync(),
                UrunSayisi = await _context.Urunlers.CountAsync(),
                YorumSayisi = await _context.Yorumlars.CountAsync(),
                KategoriSayisi = await _context.Kategorilers.CountAsync(),
                SonEklenenUrunler = await _context.Urunlers
                    .Include(u => u.EkleyenKullanici)
                    .Include(u => u.Kategori)
                    .OrderByDescending(u => u.EklenmeTarihi)
                    .Take(5)
                    .ToListAsync(),
                SonYorumlar = await _context.Yorumlars
                    .Include(y => y.Kullanici)
                    .Include(y => y.Urun)
                    .OrderByDescending(y => y.YorumTarihi)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboardViewModel);
        }

        public async Task<IActionResult> Kullanicilar()
        {
            var kullanicilar = await _context.Kullanicilars
                .OrderByDescending(k => k.KayitTarihi)
                .ToListAsync();
            
            return View(kullanicilar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KullaniciDuzenle(int id, bool isAdmin)
        {
            var kullanici = await _context.Kullanicilars.FindAsync(id);
            
            if (kullanici == null)
            {
                return NotFound();
            }

            kullanici.IsAdmin = isAdmin;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Kullanicilar));
        }

        public async Task<IActionResult> KullaniciDetay(int id)
        {
            var kullanici = await _context.Kullanicilars
                .Include(k => k.Urunlers)
                    .ThenInclude(u => u.Kategori)
                .Include(k => k.Yorumlars)
                    .ThenInclude(y => y.Urun)
                .Include(k => k.Puanlars)
                    .ThenInclude(p => p.Urun)
                .FirstOrDefaultAsync(k => k.KullaniciId == id);

            if (kullanici == null)
            {
                return NotFound();
            }

            return View(kullanici);
        }

        [HttpPost]
        [Route("Admin/KullaniciDuzenle/Admin")] // Explicit route for admin role update
        public async Task<IActionResult> KullaniciDuzenleAdmin(int id, bool? isAdmin)
        {
            var kullanici = await _context.Kullanicilars.FindAsync(id);
            
            if (kullanici == null)
            {
                return NotFound();
            }


                kullanici.IsAdmin = isAdmin.Value;
                await _context.SaveChangesAsync();
            
            
            return RedirectToAction(nameof(Kullanicilar));
        }

        [HttpPost]
        [Route("Admin/KullaniciDuzenle")] // Explicit route for full edit
        public async Task<IActionResult> KullaniciDuzenle(int id, [Bind("KullaniciId,AdSoyad,Email,IsAdmin,KayitTarihi")] Kullanicilar kullanici, bool? isAdmin = null)
        {
            if (id != kullanici.KullaniciId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingKullanici = await _context.Kullanicilars.FindAsync(id);
                    
                    if (existingKullanici == null)
                    {
                        return NotFound();
                    }

                    // Update only the fields that can be changed
                    existingKullanici.AdSoyad = kullanici.AdSoyad;
                    existingKullanici.Email = kullanici.Email;
                    
                    // Update admin status if provided
                    if (isAdmin.HasValue)
                    {
                        existingKullanici.IsAdmin = isAdmin.Value;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Kullanicilar));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KullaniciExists(kullanici.KullaniciId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(kullanici);
        }

        public async Task<IActionResult> Urunler()
        {
            var urunler = await _context.Urunlers
                .Include(u => u.EkleyenKullanici)
                .Include(u => u.Kategori)
                .OrderByDescending(u => u.EklenmeTarihi)
                .ToListAsync();
            
            return View(urunler);
        }

        public async Task<IActionResult> UrunSil(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = await _context.Urunlers
                .Include(u => u.Puanlars)
                .Include(u => u.Yorumlars)
                .Include(u=>u.EkleyenKullanici)
                .Include(u=>u.Kategori)
                .FirstOrDefaultAsync(m => m.UrunId == id);

            if (urun == null)
            {
                return NotFound();
            }

            return View(urun);
        }

        [HttpPost, ActionName("UrunSil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrunSilConfirmed(int id)
        {
            var urun = await _context.Urunlers
                .Include(u => u.Puanlars)
                .Include(u => u.Yorumlars)
                .Include(u => u.EkleyenKullanici)
                .Include(u => u.Kategori)
                .FirstOrDefaultAsync(m => m.UrunId == id);

            if (urun == null)
            {
                return NotFound();
            }

            try
            {
                // Delete related Yorumlar records first
                if (urun.Yorumlars != null)
                {
                    foreach (var yorum in urun.Yorumlars)
                    {
                        _context.Yorumlars.Remove(yorum);
                    }
                }

                // Delete related Puanlar records
                if (urun.Puanlars != null)
                {
                    foreach (var puan in urun.Puanlars)
                    {
                        _context.Puanlars.Remove(puan);
                    }
                }

                // Delete product image if exists
                if (!string.IsNullOrEmpty(urun.ResimUrl))
                {
                    // Get the physical path of the image
                    var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", urun.ResimUrl.TrimStart('/'));
                    
                    // Check if the file exists
                    if (System.IO.File.Exists(physicalPath))
                    {
                        // Delete the file
                        System.IO.File.Delete(physicalPath);
                    }
                }

                _context.Urunlers.Remove(urun);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Urunler));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ürün silinirken bir hata oluştu: " + ex.Message);
                return View(urun);
            }
        }

        public async Task<IActionResult> UrunDuzenle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = await _context.Urunlers
                .Include(u => u.Kategori)
                .FirstOrDefaultAsync(m => m.UrunId == id);

            if (urun == null)
            {
                return NotFound();
            }

            ViewData["KategoriId"] = new SelectList(_context.Kategorilers, "KategoriId", "KategoriAdi", urun.KategoriId);
            return View(urun);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrunDuzenle(int id, [Bind("UrunId,UrunAdi,Aciklama,KategoriId,EklenmeTarihi")] Urunler urun, IFormFile UrunResim, bool SilResim = false)
        {
            if (id != urun.UrunId)
            {
                return NotFound();
            }

            try
            {
                var existingUrun = await _context.Urunlers
                    .Include(u => u.Kategori)
                    .FirstOrDefaultAsync(u => u.UrunId == id);

                if (existingUrun == null)
                {
                    return NotFound();
                }

                // Handle image operations
                if (SilResim && !string.IsNullOrEmpty(existingUrun.ResimUrl))
                {
                    _context.Database.ExecuteSqlRaw($"EXEC xp_delete_file 0, '{existingUrun.ResimUrl}'");
                    existingUrun.ResimUrl = null;
                }
                else if (UrunResim != null)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingUrun.ResimUrl))
                    {
                        _context.Database.ExecuteSqlRaw($"EXEC xp_delete_file 0, '{existingUrun.ResimUrl}'");
                    }

                    // Upload new image
                    var fileName = Path.GetFileName(UrunResim.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "urunler", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await UrunResim.CopyToAsync(stream);
                    }
                    existingUrun.ResimUrl = $"/images/urunler/{fileName}";
                }

                // Update other fields
                existingUrun.UrunAdi = urun.UrunAdi;
                existingUrun.Aciklama = urun.Aciklama;
                existingUrun.KategoriId = urun.KategoriId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Urunler));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UrunExists(urun.UrunId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ürün güncellenirken bir hata oluştu: " + ex.Message);
            }

            ViewData["KategoriId"] = new SelectList(_context.Kategorilers, "KategoriId", "KategoriAdi", urun.KategoriId);
            return View(urun);
        }

        private bool UrunExists(int id)
        {
            return _context.Urunlers.Any(e => e.UrunId == id);
        }

        public async Task<IActionResult> KategoriDuzenle(int id)
        {
            var kategori = await _context.Kategorilers.FindAsync(id);
            if (kategori == null)
            {
                return NotFound();
            }

            return View(kategori);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KategoriDuzenle([Bind("KategoriId,KategoriAdi")] Kategoriler kategori)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kategori);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Kategoriler));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KategoriExists(kategori.KategoriId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(kategori);
        }

        private bool KategoriExists(int id)
        {
            return _context.Kategorilers.Any(e => e.KategoriId == id);
        }

        private bool KullaniciExists(int id)
        {
            return _context.Kullanicilars.Any(e => e.KullaniciId == id);
        }

        public async Task<IActionResult> Yorumlar()
        {
            var yorumlar = await _context.Yorumlars
                .Include(y => y.Kullanici)
                .Include(y => y.Urun)
                    .ThenInclude(u => u.Kategori)
                .OrderByDescending(y => y.YorumTarihi)
                .ToListAsync();
            
            return View(yorumlar);
        }

        [HttpPost]
        public async Task<IActionResult> YorumSil(int id)
        {
            var yorum = await _context.Yorumlars.FindAsync(id);
            
            if (yorum == null)
            {
                return NotFound();
            }

            _context.Yorumlars.Remove(yorum);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Yorumlar));
        }

        public async Task<IActionResult> Kategoriler()
        {
            var kategoriler = await _context.Kategorilers
                .Include(k => k.Urunlers)
                .ToListAsync();
            
            return View(kategoriler);
        }

        [HttpGet]
        public IActionResult KategoriEkle()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KategoriEkle(Kategoriler kategori)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kategori);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Kategoriler));
            }
            return View(kategori);
        }

        [HttpPost]
        public async Task<IActionResult> KategoriSil(int id)
        {
            var kategori = await _context.Kategorilers.FindAsync(id);
            
            if (kategori == null)
            {
                return NotFound();
            }

            _context.Kategorilers.Remove(kategori);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Kategoriler));
        }
    }
}
