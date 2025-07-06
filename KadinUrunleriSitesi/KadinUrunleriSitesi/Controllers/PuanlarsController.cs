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
    public class PuanlarsController : Controller
    {
        private readonly KadinUrunPlatformuContext _context;

        public PuanlarsController(KadinUrunPlatformuContext context)
        {
            _context = context;
        }

        // GET: Puanlars
        public async Task<IActionResult> Index()
        {
            var kadinUrunPlatformuContext = _context.Puanlars.Include(p => p.Kullanici).Include(p => p.Urun);
            return View(await kadinUrunPlatformuContext.ToListAsync());
        }

        // GET: Puanlars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var puanlar = await _context.Puanlars
                .Include(p => p.Kullanici)
                .Include(p => p.Urun)
                .FirstOrDefaultAsync(m => m.PuanId == id);
            if (puanlar == null)
            {
                return NotFound();
            }

            return View(puanlar);
        }

        // GET: Puanlars/Create
        public IActionResult Create()
        {
            ViewData["KullaniciId"] = new SelectList(_context.Kullanicilars, "KullaniciId", "KullaniciId");
            ViewData["UrunId"] = new SelectList(_context.Urunlers, "UrunId", "UrunId");
            return View();
        }

        // POST: Puanlars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PuanId,UrunId,KullaniciId,Puan")] Puanlar puanlar)
        {
            if (ModelState.IsValid)
            {
                _context.Add(puanlar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KullaniciId"] = new SelectList(_context.Kullanicilars, "KullaniciId", "KullaniciId", puanlar.KullaniciId);
            ViewData["UrunId"] = new SelectList(_context.Urunlers, "UrunId", "UrunId", puanlar.UrunId);
            return View(puanlar);
        }

        // GET: Puanlars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var puanlar = await _context.Puanlars.FindAsync(id);
            if (puanlar == null)
            {
                return NotFound();
            }
            ViewData["KullaniciId"] = new SelectList(_context.Kullanicilars, "KullaniciId", "KullaniciId", puanlar.KullaniciId);
            ViewData["UrunId"] = new SelectList(_context.Urunlers, "UrunId", "UrunId", puanlar.UrunId);
            return View(puanlar);
        }

        // POST: Puanlars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PuanId,UrunId,KullaniciId,Puan")] Puanlar puanlar)
        {
            if (id != puanlar.PuanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(puanlar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PuanlarExists(puanlar.PuanId))
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
            ViewData["KullaniciId"] = new SelectList(_context.Kullanicilars, "KullaniciId", "KullaniciId", puanlar.KullaniciId);
            ViewData["UrunId"] = new SelectList(_context.Urunlers, "UrunId", "UrunId", puanlar.UrunId);
            return View(puanlar);
        }

        // GET: Puanlars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var puanlar = await _context.Puanlars
                .Include(p => p.Kullanici)
                .Include(p => p.Urun)
                .FirstOrDefaultAsync(m => m.PuanId == id);
            if (puanlar == null)
            {
                return NotFound();
            }

            return View(puanlar);
        }

        // POST: Puanlars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var puanlar = await _context.Puanlars.FindAsync(id);
            if (puanlar != null)
            {
                _context.Puanlars.Remove(puanlar);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PuanlarExists(int id)
        {
            return _context.Puanlars.Any(e => e.PuanId == id);
        }
    }
}
