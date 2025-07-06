using System;
using System.Collections.Generic;

namespace KadinUrunleriSitesi.Models;

public partial class Urunler
{
    public int UrunId { get; set; }

    public string UrunAdi { get; set; } = null!;

    public string? Aciklama { get; set; }

    public string? ResimUrl { get; set; }

    public int KategoriId { get; set; }

    public int EkleyenKullaniciId { get; set; }

    public DateTime? EklenmeTarihi { get; set; }

    public virtual Kullanicilar EkleyenKullanici { get; set; } = null!;

    public virtual Kategoriler Kategori { get; set; } = null!;

    public virtual ICollection<Puanlar> Puanlars { get; set; } = new List<Puanlar>();

    public virtual ICollection<Yorumlar> Yorumlars { get; set; } = new List<Yorumlar>();
}
