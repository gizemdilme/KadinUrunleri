using System;
using System.Collections.Generic;

namespace KadinUrunleriSitesi.Models;

public partial class Yorumlar
{
    public int YorumId { get; set; }

    public int UrunId { get; set; }

    public int KullaniciId { get; set; }

    public string Icerik { get; set; } = null!;

    public DateTime? YorumTarihi { get; set; }

    public string? YorumFotoUrl { get; set; }

    public virtual Kullanicilar Kullanici { get; set; } = null!;

    public virtual Urunler Urun { get; set; } = null!;
}
