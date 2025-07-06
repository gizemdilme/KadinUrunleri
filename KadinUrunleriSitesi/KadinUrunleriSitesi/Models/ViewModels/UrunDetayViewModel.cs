using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace KadinUrunleriSitesi.Models.ViewModels
{
    public class UrunDetayViewModel
    {
        public Urunler Urun { get; set; } = null!;
        public List<Yorumlar> Yorumlar { get; set; } = new List<Yorumlar>();
        public double OrtalamaPuan { get; set; }
        public int PuanSayisi { get; set; } // Number of ratings
        public int KullaniciPuani { get; set; } // Current user's rating (0 if not rated)
        public YorumEkleViewModel YeniYorum { get; set; } = new YorumEkleViewModel();
    }

    public class YorumEkleViewModel
    {
        public int UrunId { get; set; }
        public string Icerik { get; set; } = string.Empty;
        public IFormFile? YorumResim { get; set; }
    }
}
