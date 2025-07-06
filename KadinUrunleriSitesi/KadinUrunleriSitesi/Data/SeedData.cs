using KadinUrunleriSitesi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KadinUrunleriSitesi.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new KadinUrunPlatformuContext(
                serviceProvider.GetRequiredService<DbContextOptions<KadinUrunPlatformuContext>>()))
            {
                // We'll check each item individually before adding

                // Add users if they don't exist
                var adminUser = context.Kullanicilars.FirstOrDefault(k => k.Email == "admin@example.com");
                if (adminUser == null)
                {
                    adminUser = new Kullanicilar
                    {
                        AdSoyad = "Admin User",
                        Email = "admin@example.com",
                        Sifre = "admin123", // In a real application, this should be hashed
                        KayitTarihi = DateTime.Now,
                        IsAdmin = true,
                        ProfilFotoUrl = "/images/users/admin-profile.jpg"
                    };
                    context.Kullanicilars.Add(adminUser);
                    context.SaveChanges();
                }

                var normalUser1 = context.Kullanicilars.FirstOrDefault(k => k.Email == "ayse@example.com");
                if (normalUser1 == null)
                {
                    normalUser1 = new Kullanicilar
                    {
                        AdSoyad = "Ayşe Yılmaz",
                        Email = "ayse@example.com",
                        Sifre = "user123",
                        KayitTarihi = DateTime.Now.AddDays(-10),
                        IsAdmin = false,
                        ProfilFotoUrl = "/images/users/user1-profile.jpg"
                    };
                    context.Kullanicilars.Add(normalUser1);
                    context.SaveChanges();
                }

                var normalUser2 = context.Kullanicilars.FirstOrDefault(k => k.Email == "fatma@example.com");
                if (normalUser2 == null)
                {
                    normalUser2 = new Kullanicilar
                    {
                        AdSoyad = "Fatma Demir",
                        Email = "fatma@example.com",
                        Sifre = "user123",
                        KayitTarihi = DateTime.Now.AddDays(-7),
                        IsAdmin = false,
                        ProfilFotoUrl = "/images/users/user2-profile.jpg"
                    };
                    context.Kullanicilars.Add(normalUser2);
                    context.SaveChanges();
                }

                var normalUser3 = context.Kullanicilars.FirstOrDefault(k => k.Email == "zeynep@example.com");
                if (normalUser3 == null)
                {
                    normalUser3 = new Kullanicilar
                    {
                        AdSoyad = "Zeynep Kaya",
                        Email = "zeynep@example.com",
                        Sifre = "user123",
                        KayitTarihi = DateTime.Now.AddDays(-5),
                        IsAdmin = false,
                        ProfilFotoUrl = "/images/users/user3-profile.jpg"
                    };
                    context.Kullanicilars.Add(normalUser3);
                    context.SaveChanges();
                }

                // Add categories if they don't exist
                var kategori1 = context.Kategorilers.FirstOrDefault(k => k.KategoriAdi == "Makyaj Ürünleri");
                if (kategori1 == null)
                {
                    kategori1 = new Kategoriler { KategoriAdi = "Makyaj Ürünleri" };
                    context.Kategorilers.Add(kategori1);
                    context.SaveChanges();
                }

                var kategori2 = context.Kategorilers.FirstOrDefault(k => k.KategoriAdi == "Cilt Bakımı");
                if (kategori2 == null)
                {
                    kategori2 = new Kategoriler { KategoriAdi = "Cilt Bakımı" };
                    context.Kategorilers.Add(kategori2);
                    context.SaveChanges();
                }

                var kategori3 = context.Kategorilers.FirstOrDefault(k => k.KategoriAdi == "Saç Bakımı");
                if (kategori3 == null)
                {
                    kategori3 = new Kategoriler { KategoriAdi = "Saç Bakımı" };
                    context.Kategorilers.Add(kategori3);
                    context.SaveChanges();
                }

                var kategori4 = context.Kategorilers.FirstOrDefault(k => k.KategoriAdi == "Parfüm & Deodorant");
                if (kategori4 == null)
                {
                    kategori4 = new Kategoriler { KategoriAdi = "Parfüm & Deodorant" };
                    context.Kategorilers.Add(kategori4);
                    context.SaveChanges();
                }

                var kategori5 = context.Kategorilers.FirstOrDefault(k => k.KategoriAdi == "Aksesuar");
                if (kategori5 == null)
                {
                    kategori5 = new Kategoriler { KategoriAdi = "Aksesuar" };
                    context.Kategorilers.Add(kategori5);
                    context.SaveChanges();
                }

                // Add products if they don't exist
                var urun1 = context.Urunlers.FirstOrDefault(u => u.UrunAdi == "Nemlendirici Krem" && u.EkleyenKullaniciId == normalUser1.KullaniciId);
                if (urun1 == null)
                {
                    urun1 = new Urunler
                    {
                        UrunAdi = "Nemlendirici Krem",
                        Aciklama = "Kuru ciltler için 24 saat nemlendirici etkili bakım kremi. Cildinizi besler ve nemlendirir.",
                        ResimUrl = "/images/products/nemlendirici-krem.jpg",
                        EklenmeTarihi = DateTime.Now.AddDays(-6),
                        EkleyenKullaniciId = normalUser1.KullaniciId,
                        KategoriId = kategori2.KategoriId
                    };
                    context.Urunlers.Add(urun1);
                    context.SaveChanges();
                }

                var urun2 = context.Urunlers.FirstOrDefault(u => u.UrunAdi == "Mat Ruj" && u.EkleyenKullaniciId == normalUser2.KullaniciId);
                if (urun2 == null)
                {
                    urun2 = new Urunler
                    {
                        UrunAdi = "Mat Ruj",
                        Aciklama = "Uzun süre kalıcı, mat bitişli, dudakları kurutmayan kadife dokulu ruj. Gün boyu tazeliğini korur.",
                        ResimUrl = "/images/products/mat-ruj.jpg",
                        EklenmeTarihi = DateTime.Now.AddDays(-5),
                        EkleyenKullaniciId = normalUser2.KullaniciId,
                        KategoriId = kategori1.KategoriId
                    };
                    context.Urunlers.Add(urun2);
                    context.SaveChanges();
                }

                var urun3 = context.Urunlers.FirstOrDefault(u => u.UrunAdi == "Saç Bakım Serumu" && u.EkleyenKullaniciId == normalUser3.KullaniciId);
                if (urun3 == null)
                {
                    urun3 = new Urunler
                    {
                        UrunAdi = "Saç Bakım Serumu",
                        Aciklama = "Yıpranmış saçlar için onarıcı ve koruyucu etkili serum. Saçlarınızı besler ve parlaklık kazandırır.",
                        ResimUrl = "/images/products/sac-serumu.jpg",
                        EklenmeTarihi = DateTime.Now.AddDays(-4),
                        EkleyenKullaniciId = normalUser3.KullaniciId,
                        KategoriId = kategori3.KategoriId
                    };
                    context.Urunlers.Add(urun3);
                    context.SaveChanges();
                }

                var urun4 = context.Urunlers.FirstOrDefault(u => u.UrunAdi == "Çiçek Kokulu Parfüm" && u.EkleyenKullaniciId == normalUser1.KullaniciId);
                if (urun4 == null)
                {
                    urun4 = new Urunler
                    {
                        UrunAdi = "Çiçek Kokulu Parfüm",
                        Aciklama = "Yasemin ve gül notaları içeren, kalıcı ve ferah kokusuyla gün boyu tazelik hissi veren kadın parfümü.",
                        ResimUrl = "/images/products/parfum.jpg",
                        EklenmeTarihi = DateTime.Now.AddDays(-3),
                        EkleyenKullaniciId = normalUser1.KullaniciId,
                        KategoriId = kategori4.KategoriId
                    };
                    context.Urunlers.Add(urun4);
                    context.SaveChanges();
                }

                var urun5 = context.Urunlers.FirstOrDefault(u => u.UrunAdi == "Gümüş Kolye" && u.EkleyenKullaniciId == normalUser2.KullaniciId);
                if (urun5 == null)
                {
                    urun5 = new Urunler
                    {
                        UrunAdi = "Gümüş Kolye",
                        Aciklama = "925 ayar gümüş, zarif tasarımlı, her kıyafete uyum sağlayan şık kolye.",
                        ResimUrl = "/images/products/kolye.jpg",
                        EklenmeTarihi = DateTime.Now.AddDays(-2),
                        EkleyenKullaniciId = normalUser2.KullaniciId,
                        KategoriId = kategori5.KategoriId
                    };
                    context.Urunlers.Add(urun5);
                    context.SaveChanges();
                }

                var urun6 = context.Urunlers.FirstOrDefault(u => u.UrunAdi == "Göz Farı Paleti" && u.EkleyenKullaniciId == normalUser3.KullaniciId);
                if (urun6 == null)
                {
                    urun6 = new Urunler
                    {
                        UrunAdi = "Göz Farı Paleti",
                        Aciklama = "12 farklı renk seçeneği ile hem gündüz hem gece kullanımına uygun, kalıcı ve pigmentasyonu yüksek göz farı paleti.",
                        ResimUrl = "/images/products/goz-fari.jpg",
                        EklenmeTarihi = DateTime.Now.AddDays(-1),
                        EkleyenKullaniciId = normalUser3.KullaniciId,
                        KategoriId = kategori1.KategoriId
                    };
                    context.Urunlers.Add(urun6);
                    context.SaveChanges();
                }

                // Add comments if they don't exist
                if (!context.Yorumlars.Any(y => y.UrunId == urun1.UrunId && y.KullaniciId == normalUser2.KullaniciId))
                {
                    var yorum1 = new Yorumlar
                    {
                        UrunId = urun1.UrunId,
                        KullaniciId = normalUser2.KullaniciId,
                        Icerik = "Bu kremi bir aydır kullanıyorum ve cildim çok daha nemli ve yumuşak hissediyor. Kesinlikle tavsiye ederim!",
                        YorumTarihi = DateTime.Now.AddDays(-5),
                        YorumFotoUrl = "/images/comments/yorum1.jpg"
                    };
                    context.Yorumlars.Add(yorum1);
                    context.SaveChanges();
                }

                if (!context.Yorumlars.Any(y => y.UrunId == urun1.UrunId && y.KullaniciId == normalUser3.KullaniciId))
                {
                    var yorum2 = new Yorumlar
                    {
                        UrunId = urun1.UrunId,
                        KullaniciId = normalUser3.KullaniciId,
                        Icerik = "Kuru cildim için harika bir ürün oldu. Kokusu da çok güzel.",
                        YorumTarihi = DateTime.Now.AddDays(-4),
                        YorumFotoUrl = null
                    };
                    context.Yorumlars.Add(yorum2);
                    context.SaveChanges();
                }

                if (!context.Yorumlars.Any(y => y.UrunId == urun2.UrunId && y.KullaniciId == normalUser1.KullaniciId))
                {
                    var yorum3 = new Yorumlar
                    {
                        UrunId = urun2.UrunId,
                        KullaniciId = normalUser1.KullaniciId,
                        Icerik = "Rengi çok güzel ve gerçekten kalıcı. Dudaklarımı kurutmuyor, çok memnun kaldım.",
                        YorumTarihi = DateTime.Now.AddDays(-3),
                        YorumFotoUrl = "/images/comments/yorum3.jpg"
                    };
                    context.Yorumlars.Add(yorum3);
                    context.SaveChanges();
                }

                if (!context.Yorumlars.Any(y => y.UrunId == urun3.UrunId && y.KullaniciId == normalUser2.KullaniciId))
                {
                    var yorum4 = new Yorumlar
                    {
                        UrunId = urun3.UrunId,
                        KullaniciId = normalUser2.KullaniciId,
                        Icerik = "Saçlarım boyalı ve yıpranmıştı. Bu serum ile çok daha sağlıklı görünüyor.",
                        YorumTarihi = DateTime.Now.AddDays(-2),
                        YorumFotoUrl = "/images/comments/yorum4.jpg"
                    };
                    context.Yorumlars.Add(yorum4);
                    context.SaveChanges();
                }

                if (!context.Yorumlars.Any(y => y.UrunId == urun4.UrunId && y.KullaniciId == normalUser3.KullaniciId))
                {
                    var yorum5 = new Yorumlar
                    {
                        UrunId = urun4.UrunId,
                        KullaniciId = normalUser3.KullaniciId,
                        Icerik = "Kokusu harika ve kalıcılığı çok iyi. Tüm gün üzerimde hissediyorum.",
                        YorumTarihi = DateTime.Now.AddDays(-1),
                        YorumFotoUrl = null
                    };
                    context.Yorumlars.Add(yorum5);
                    context.SaveChanges();
                }

                if (!context.Yorumlars.Any(y => y.UrunId == urun5.UrunId && y.KullaniciId == normalUser1.KullaniciId))
                {
                    var yorum6 = new Yorumlar
                    {
                        UrunId = urun5.UrunId,
                        KullaniciId = normalUser1.KullaniciId,
                        Icerik = "Kolye çok zarif ve şık. Her kıyafetle kombinleyebiliyorum.",
                        YorumTarihi = DateTime.Now.AddHours(-12),
                        YorumFotoUrl = "/images/comments/yorum6.jpg"
                    };
                    context.Yorumlars.Add(yorum6);
                    context.SaveChanges();
                }

                if (!context.Yorumlars.Any(y => y.UrunId == urun6.UrunId && y.KullaniciId == normalUser2.KullaniciId))
                {
                    var yorum7 = new Yorumlar
                    {
                        UrunId = urun6.UrunId,
                        KullaniciId = normalUser2.KullaniciId,
                        Icerik = "Renkleri çok pigmentli ve karıştırması kolay. Günlük makyajım için vazgeçilmez oldu.",
                        YorumTarihi = DateTime.Now.AddHours(-6),
                        YorumFotoUrl = "/images/comments/yorum7.jpg"
                    };
                    context.Yorumlars.Add(yorum7);
                    context.SaveChanges();
                }

                // Add ratings if they don't exist
                // Rating 1
                if (!context.Puanlars.Any(p => p.UrunId == urun1.UrunId && p.KullaniciId == normalUser2.KullaniciId))
                {
                    context.Puanlars.Add(new Puanlar { UrunId = urun1.UrunId, KullaniciId = normalUser2.KullaniciId, Puan = 5 });
                    context.SaveChanges();
                }
                
                // Rating 2
                if (!context.Puanlars.Any(p => p.UrunId == urun1.UrunId && p.KullaniciId == normalUser3.KullaniciId))
                {
                    context.Puanlars.Add(new Puanlar { UrunId = urun1.UrunId, KullaniciId = normalUser3.KullaniciId, Puan = 4 });
                    context.SaveChanges();
                }
                
                // Rating 3
                if (!context.Puanlars.Any(p => p.UrunId == urun2.UrunId && p.KullaniciId == normalUser1.KullaniciId))
                {
                    context.Puanlars.Add(new Puanlar { UrunId = urun2.UrunId, KullaniciId = normalUser1.KullaniciId, Puan = 5 });
                    context.SaveChanges();
                }
                
                // Rating 4
                if (!context.Puanlars.Any(p => p.UrunId == urun2.UrunId && p.KullaniciId == normalUser3.KullaniciId))
                {
                    context.Puanlars.Add(new Puanlar { UrunId = urun2.UrunId, KullaniciId = normalUser3.KullaniciId, Puan = 4 });
                    context.SaveChanges();
                }
                
                // Rating 5
                if (!context.Puanlars.Any(p => p.UrunId == urun3.UrunId && p.KullaniciId == normalUser1.KullaniciId))
                {
                    context.Puanlars.Add(new Puanlar { UrunId = urun3.UrunId, KullaniciId = normalUser1.KullaniciId, Puan = 3 });
                    context.SaveChanges();
                }
                
                // Rating 6
                if (!context.Puanlars.Any(p => p.UrunId == urun3.UrunId && p.KullaniciId == normalUser2.KullaniciId))
                {
                    context.Puanlars.Add(new Puanlar { UrunId = urun3.UrunId, KullaniciId = normalUser2.KullaniciId, Puan = 4 });
                    context.SaveChanges();
                }
                
                // Rating 7
                if (!context.Puanlars.Any(p => p.UrunId == urun4.UrunId && p.KullaniciId == normalUser2.KullaniciId))
                {
                    context.Puanlars.Add(new Puanlar { UrunId = urun4.UrunId, KullaniciId = normalUser2.KullaniciId, Puan = 5 });
                    context.SaveChanges();
                }
                
                // Rating 8
                if (!context.Puanlars.Any(p => p.UrunId == urun4.UrunId && p.KullaniciId == normalUser3.KullaniciId))
                {
                    context.Puanlars.Add(new Puanlar { UrunId = urun4.UrunId, KullaniciId = normalUser3.KullaniciId, Puan = 5 });
                    context.SaveChanges();
                }
                
                // Rating 9
                if (!context.Puanlars.Any(p => p.UrunId == urun5.UrunId && p.KullaniciId == normalUser1.KullaniciId))
                {
                    context.Puanlars.Add(new Puanlar { UrunId = urun5.UrunId, KullaniciId = normalUser1.KullaniciId, Puan = 4 });
                    context.SaveChanges();
                }
                
                // Rating 10
                if (!context.Puanlars.Any(p => p.UrunId == urun5.UrunId && p.KullaniciId == normalUser3.KullaniciId))
                {
                    context.Puanlars.Add(new Puanlar { UrunId = urun5.UrunId, KullaniciId = normalUser3.KullaniciId, Puan = 3 });
                    context.SaveChanges();
                }
                
                // Rating 11
                if (!context.Puanlars.Any(p => p.UrunId == urun6.UrunId && p.KullaniciId == normalUser1.KullaniciId))
                {
                    context.Puanlars.Add(new Puanlar { UrunId = urun6.UrunId, KullaniciId = normalUser1.KullaniciId, Puan = 5 });
                    context.SaveChanges();
                }
                
                // Rating 12
                if (!context.Puanlars.Any(p => p.UrunId == urun6.UrunId && p.KullaniciId == normalUser2.KullaniciId))
                {
                    context.Puanlars.Add(new Puanlar { UrunId = urun6.UrunId, KullaniciId = normalUser2.KullaniciId, Puan = 4 });
                    context.SaveChanges();
                }
            }
        }
    }
}
