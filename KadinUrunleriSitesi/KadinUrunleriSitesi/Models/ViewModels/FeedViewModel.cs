using System;
using System.Collections.Generic;

namespace KadinUrunleriSitesi.Models.ViewModels
{
    public class FeedViewModel
    {
        public List<UrunFeedItem> Urunler { get; set; } = new List<UrunFeedItem>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? Search { get; set; }  

    }

    public class UrunFeedItem
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string? ResimUrl { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public string EkleyenKullaniciAdi { get; set; } = string.Empty;
        public string? EkleyenKullaniciProfilFoto { get; set; }
        public int KullaniciId { get; set; }
        public double OrtalamaPuan { get; set; }
        public int PuanSayisi { get; set; }
        public int YorumSayisi { get; set; }
        public List<YorumViewModel> SonYorumlar { get; set; } = new List<YorumViewModel>();
    }

    public class YorumViewModel
    {
        public int YorumId { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Icerik { get; set; } = string.Empty;
        public string? YorumFotoUrl { get; set; }
        public DateTime YorumTarihi { get; set; }
    }
}
