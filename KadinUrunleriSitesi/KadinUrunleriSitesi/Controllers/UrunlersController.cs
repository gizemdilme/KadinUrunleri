using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using KadinUrunleriSitesi.Models;
using KadinUrunleriSitesi.Services;

namespace KadinUrunleriSitesi.Controllers
{
    public class UrunlersController : Controller
    {
        private readonly KadinUrunPlatformuContext _context;
        private readonly FileUploadService _fileUploadService;

        public UrunlersController(KadinUrunPlatformuContext context, FileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        // GET: Urunlers
        public async Task<IActionResult> Index(string? arama)
        {
            var userId = User.FindFirstValue("KullaniciID");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int currentUserId))
            {
                return RedirectToAction("Giris", "Login");
            }

            var urunler = _context.Urunlers
                .Include(u => u.EkleyenKullanici)
                .Include(u => u.Kategori)
                .Include(u => u.Puanlars)
                .Include(u => u.Yorumlars)
                .Where(u => u.EkleyenKullaniciId == currentUserId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(arama))
            {
                urunler = urunler.Where(u =>
                    u.UrunAdi.Contains(arama) ||
                    u.Aciklama.Contains(arama) ||
                    u.Kategori.KategoriAdi.Contains(arama));
            }

            return View(await urunler.OrderByDescending(u => u.EklenmeTarihi).ToListAsync());
        }

        // GET: Urunlers/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue("KullaniciID");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int currentUserId))
            {
                return RedirectToAction("Giris", "Login");
            }

            var urun = new Urunler
            {
                EkleyenKullaniciId = currentUserId,
                EklenmeTarihi = DateTime.Now
            };

            ViewData["KategoriId"] = new SelectList(_context.Kategorilers, "KategoriId", "KategoriAdi");
            return View(urun);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UrunId,UrunAdi,Aciklama,KategoriId,EkleyenKullaniciId")] Urunler urunler, IFormFile UrunResim)
        {
            var userId = User.FindFirstValue("KullaniciID");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int currentUserId))
            {
                return RedirectToAction("Giris", "Login");
            }

            urunler.EkleyenKullaniciId = currentUserId;
            urunler.EklenmeTarihi = DateTime.Now;

            // 🔴 En kritik yer burası! Eğer resim varsa yükle ve yolu kaydet!
            if (UrunResim != null && UrunResim.Length > 0)
            {
                var yol = await _fileUploadService.UploadFileAsync(UrunResim, "images/products");
                urunler.ResimUrl = yol;
            }

            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(urunler.UrunAdi))
                ModelState.AddModelError("UrunAdi", "Ürün adı gereklidir.");

            if (urunler.KategoriId <= 0)
                ModelState.AddModelError("KategoriId", "Kategori seçiniz.");

            if (ModelState.IsValid)
            {
                _context.Add(urunler);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["KategoriId"] = new SelectList(_context.Kategorilers, "KategoriId", "KategoriAdi", urunler.KategoriId);
            return View(urunler);
        }

        // GET: Urunlers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var urunler = await _context.Urunlers.FindAsync(id);
            if (urunler == null)
                return NotFound();

            ViewData["KategoriId"] = new SelectList(_context.Kategorilers, "KategoriId", "KategoriAdi", urunler.KategoriId);
            return View(urunler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UrunId,UrunAdi,Aciklama,KategoriId,EkleyenKullaniciId,EklenmeTarihi,ResimUrl")] Urunler urunler, IFormFile UrunResim)
        {
            if (id != urunler.UrunId)
                return NotFound();

            
            var existingUrun = await _context.Urunlers.AsNoTracking().FirstOrDefaultAsync(u => u.UrunId == id);
            if (existingUrun == null)
                return NotFound();

            
            if (UrunResim != null && UrunResim.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingUrun.ResimUrl))
                    _fileUploadService.DeleteFile(existingUrun.ResimUrl);

                var yeniYol = await _fileUploadService.UploadFileAsync(UrunResim, "images/products");
                urunler.ResimUrl = yeniYol;
            }
            else
            {
                
                urunler.ResimUrl = existingUrun.ResimUrl;
            }

            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(urunler.UrunAdi))
                ModelState.AddModelError("UrunAdi", "Ürün adı gereklidir.");

            if (urunler.KategoriId <= 0)
                ModelState.AddModelError("KategoriId", "Geçerli bir kategori seçiniz.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(urunler);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Urunlers.Any(e => e.UrunId == urunler.UrunId))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["KategoriId"] = new SelectList(_context.Kategorilers, "KategoriId", "KategoriAdi", urunler.KategoriId);
            return View(urunler);
        }

        // GET: Urunlers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var urunler = await _context.Urunlers
                .Include(u => u.EkleyenKullanici)
                .Include(u => u.Kategori)
                .FirstOrDefaultAsync(m => m.UrunId == id);

            if (urunler == null)
                return NotFound();

            return View(urunler);
        }

        // POST: Urunlers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var urunler = await _context.Urunlers
                .Include(u => u.Puanlars)
                .Include(u => u.Yorumlars)
                .FirstOrDefaultAsync(u => u.UrunId == id);

            if (urunler != null)
            {
                if (urunler.Yorumlars != null)
                    _context.Yorumlars.RemoveRange(urunler.Yorumlars);

                if (urunler.Puanlars != null)
                    _context.Puanlars.RemoveRange(urunler.Puanlars);

                if (!string.IsNullOrEmpty(urunler.ResimUrl))
                    _fileUploadService.DeleteFile(urunler.ResimUrl);

                _context.Urunlers.Remove(urunler);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UrunlerExists(int id)
        {
            return _context.Urunlers.Any(e => e.UrunId == id);
        }
    }
}
