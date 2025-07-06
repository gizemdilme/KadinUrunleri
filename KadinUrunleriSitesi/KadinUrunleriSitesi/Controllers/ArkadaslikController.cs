using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using KadinUrunleriSitesi.Models;

namespace KadinUrunleriSitesi.Controllers
{
    public class ArkadaslikController : Controller
    {
        private readonly KadinUrunPlatformuContext _context;

        public ArkadaslikController(KadinUrunPlatformuContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IstekGonder(int alanId)
        {
            var gonderenIdStr = User.FindFirstValue("KullaniciID");

            if (string.IsNullOrEmpty(gonderenIdStr) || !int.TryParse(gonderenIdStr, out int gonderenId))
                return RedirectToAction("Giris", "Login");

            if (gonderenId == alanId)
            {
                TempData["Mesaj"] = "Kendinize istek gönderemezsiniz.";
                return RedirectToAction("Detay", "Profil", new { id = alanId });
            }

            var mevcut = await _context.Arkadasliklar.FirstOrDefaultAsync(a =>
                (a.GonderenId == gonderenId && a.AlanId == alanId) ||
                (a.GonderenId == alanId && a.AlanId == gonderenId));

            if (mevcut != null)
            {
                TempData["Mesaj"] = "Bu kişiyle zaten bir isteğiniz var veya arkadaşsınız.";
                return RedirectToAction("Detay", "Profil", new { id = alanId });
            }

            var yeniIstek = new Arkadaslik
            {
                GonderenId = gonderenId,
                AlanId = alanId,
                Tarih = DateTime.Now,
                Durum = "Beklemede"
            };

            _context.Arkadasliklar.Add(yeniIstek);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Arkadaşlık isteği gönderildi.";
            return RedirectToAction("Detay", "Profil", new { id = alanId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KabulEt(int id)
        {
            var istek = await _context.Arkadasliklar.FindAsync(id);
            if (istek != null && istek.Durum == "Beklemede")
            {
                istek.Durum = "Kabul";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Bildirimler", "Profil");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reddet(int id)
        {
            var istek = await _context.Arkadasliklar.FindAsync(id);
            if (istek != null && istek.Durum == "Beklemede")
            {
                istek.Durum = "Reddet";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Bildirimler", "Profil");
        }
    }
}
