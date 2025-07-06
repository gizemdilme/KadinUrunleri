using System;
using System.Collections.Generic;

namespace KadinUrunleriSitesi.Models;

public partial class Puanlar
{
    public int PuanId { get; set; }

    public int UrunId { get; set; }

    public int KullaniciId { get; set; }

    public int Puan { get; set; }

    public virtual Kullanicilar Kullanici { get; set; } = null!;

    public virtual Urunler Urun { get; set; } = null!;
}
