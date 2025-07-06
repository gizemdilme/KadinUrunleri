using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KadinUrunleriSitesi.Models;

namespace KadinUrunleriSitesi.Controllers
{
    public class KullanicilarsController : Controller
    {
        private readonly KadinUrunPlatformuContext _context;

        public KullanicilarsController(KadinUrunPlatformuContext context)
        {
            _context = context;
        }

        // GET: Kullanicilars
        public async Task<IActionResult> Index()
        {
            return View(await _context.Kullanicilars.ToListAsync());
        }

        // GET: Kullanicilars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kullanicilar = await _context.Kullanicilars
                .FirstOrDefaultAsync(m => m.KullaniciId == id);
            if (kullanicilar == null)
            {
                return NotFound();
            }

            return View(kullanicilar);
        }

        // GET: Kullanicilars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Kullanicilars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KullaniciId,AdSoyad,Email,Sifre,KayitTarihi")] Kullanicilar kullanicilar)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kullanicilar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kullanicilar);
        }
        public async Task<IActionResult> Detay(int id)
        {
            var kullanici = await _context.Kullanicilars.FindAsync(id);
            if (kullanici == null) return NotFound();

            return View(kullanici); // Views/Kullanicilar/Detay.cshtml dosyasını açacak
        }

        // GET: Kullanicilars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kullanicilar = await _context.Kullanicilars.FindAsync(id);
            if (kullanicilar == null)
            {
                return NotFound();
            }
            return View(kullanicilar);
        }

        // POST: Kullanicilars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KullaniciId,AdSoyad,Email,Sifre,KayitTarihi")] Kullanicilar kullanicilar)
        {
            if (id != kullanicilar.KullaniciId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kullanicilar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KullanicilarExists(kullanicilar.KullaniciId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(kullanicilar);
        }

        // GET: Kullanicilars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kullanicilar = await _context.Kullanicilars
                .FirstOrDefaultAsync(m => m.KullaniciId == id);
            if (kullanicilar == null)
            {
                return NotFound();
            }

            return View(kullanicilar);
        }

        // POST: Kullanicilars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kullanicilar = await _context.Kullanicilars.FindAsync(id);
            if (kullanicilar != null)
            {
                _context.Kullanicilars.Remove(kullanicilar);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KullanicilarExists(int id)
        {
            return _context.Kullanicilars.Any(e => e.KullaniciId == id);
        }
    }
}
