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
    public class YorumlarsController : Controller
    {
        private readonly KadinUrunPlatformuContext _context;

        public YorumlarsController(KadinUrunPlatformuContext context)
        {
            _context = context;
        }

        // GET: Yorumlars
        public async Task<IActionResult> Index()
        {
            var kadinUrunPlatformuContext = _context.Yorumlars.Include(y => y.Kullanici).Include(y => y.Urun);
            return View(await kadinUrunPlatformuContext.ToListAsync());
        }

        // GET: Yorumlars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yorumlar = await _context.Yorumlars
                .Include(y => y.Kullanici)
                .Include(y => y.Urun)
                .FirstOrDefaultAsync(m => m.YorumId == id);
            if (yorumlar == null)
            {
                return NotFound();
            }

            return View(yorumlar);
        }

        // GET: Yorumlars/Create
        public IActionResult Create()
        {
            ViewData["KullaniciId"] = new SelectList(_context.Kullanicilars, "KullaniciId", "KullaniciId");
            ViewData["UrunId"] = new SelectList(_context.Urunlers, "UrunId", "UrunId");
            return View();
        }

        // POST: Yorumlars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("YorumId,UrunId,KullaniciId,Icerik,YorumTarihi")] Yorumlar yorumlar)
        {
            if (ModelState.IsValid)
            {
                _context.Add(yorumlar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KullaniciId"] = new SelectList(_context.Kullanicilars, "KullaniciId", "KullaniciId", yorumlar.KullaniciId);
            ViewData["UrunId"] = new SelectList(_context.Urunlers, "UrunId", "UrunId", yorumlar.UrunId);
            return View(yorumlar);
        }

        // GET: Yorumlars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yorumlar = await _context.Yorumlars.FindAsync(id);
            if (yorumlar == null)
            {
                return NotFound();
            }
            ViewData["KullaniciId"] = new SelectList(_context.Kullanicilars, "KullaniciId", "KullaniciId", yorumlar.KullaniciId);
            ViewData["UrunId"] = new SelectList(_context.Urunlers, "UrunId", "UrunId", yorumlar.UrunId);
            return View(yorumlar);
        }

        // POST: Yorumlars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("YorumId,UrunId,KullaniciId,Icerik,YorumTarihi")] Yorumlar yorumlar)
        {
            if (id != yorumlar.YorumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(yorumlar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!YorumlarExists(yorumlar.YorumId))
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
            ViewData["KullaniciId"] = new SelectList(_context.Kullanicilars, "KullaniciId", "KullaniciId", yorumlar.KullaniciId);
            ViewData["UrunId"] = new SelectList(_context.Urunlers, "UrunId", "UrunId", yorumlar.UrunId);
            return View(yorumlar);
        }

        // GET: Yorumlars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yorumlar = await _context.Yorumlars
                .Include(y => y.Kullanici)
                .Include(y => y.Urun)
                .FirstOrDefaultAsync(m => m.YorumId == id);
            if (yorumlar == null)
            {
                return NotFound();
            }

            return View(yorumlar);
        }

        // POST: Yorumlars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var yorumlar = await _context.Yorumlars.FindAsync(id);
            if (yorumlar != null)
            {
                _context.Yorumlars.Remove(yorumlar);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool YorumlarExists(int id)
        {
            return _context.Yorumlars.Any(e => e.YorumId == id);
        }
    }
}
