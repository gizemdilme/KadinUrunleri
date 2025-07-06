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
    public class AdminlersController : Controller
    {
        private readonly KadinUrunPlatformuContext _context;

        public AdminlersController(KadinUrunPlatformuContext context)
        {
            _context = context;
        }

        // GET: Adminlers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Adminlers.ToListAsync());
        }

        // GET: Adminlers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminler = await _context.Adminlers
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (adminler == null)
            {
                return NotFound();
            }

            return View(adminler);
        }

        // GET: Adminlers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Adminlers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,KullaniciAdi,Sifre")] Adminler adminler)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adminler);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adminler);
        }

        // GET: Adminlers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminler = await _context.Adminlers.FindAsync(id);
            if (adminler == null)
            {
                return NotFound();
            }
            return View(adminler);
        }

        // POST: Adminlers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminId,KullaniciAdi,Sifre")] Adminler adminler)
        {
            if (id != adminler.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminler);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminlerExists(adminler.AdminId))
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
            return View(adminler);
        }

        // GET: Adminlers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminler = await _context.Adminlers
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (adminler == null)
            {
                return NotFound();
            }

            return View(adminler);
        }

        // POST: Adminlers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminler = await _context.Adminlers.FindAsync(id);
            if (adminler != null)
            {
                _context.Adminlers.Remove(adminler);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminlerExists(int id)
        {
            return _context.Adminlers.Any(e => e.AdminId == id);
        }
    }
}
