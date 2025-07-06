using System;
using System.Collections.Generic;

namespace KadinUrunleriSitesi.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int KullaniciSayisi { get; set; }
        public int UrunSayisi { get; set; }
        public int YorumSayisi { get; set; }
        public int KategoriSayisi { get; set; }
        public List<Urunler> SonEklenenUrunler { get; set; } = new List<Urunler>();
        public List<Yorumlar> SonYorumlar { get; set; } = new List<Yorumlar>();
    }
}
