using System;
using System.Collections.Generic;

namespace KadinUrunleriSitesi.Models;

public partial class Kullanicilar
{
    public int KullaniciId { get; set; }

    public string AdSoyad { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Sifre { get; set; } = null!;

    public DateTime? KayitTarihi { get; set; }

    public bool? IsAdmin { get; set; } = false;

    public string? ProfilFotoUrl { get; set; }

    public virtual ICollection<Puanlar> Puanlars { get; set; } = new List<Puanlar>();

    public virtual ICollection<Urunler> Urunlers { get; set; } = new List<Urunler>();

    public virtual ICollection<Yorumlar> Yorumlars { get; set; } = new List<Yorumlar>();
}
