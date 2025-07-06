using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KadinUrunleriSitesi.Models;

public partial class KadinUrunPlatformuContext : DbContext
{
    public KadinUrunPlatformuContext()
    {
    }

    public KadinUrunPlatformuContext(DbContextOptions<KadinUrunPlatformuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adminler> Adminlers { get; set; }

    public virtual DbSet<Kategoriler> Kategorilers { get; set; }

    public virtual DbSet<Kullanicilar> Kullanicilars { get; set; }

    public virtual DbSet<Puanlar> Puanlars { get; set; }

    public virtual DbSet<Urunler> Urunlers { get; set; }

    public virtual DbSet<Yorumlar> Yorumlars { get; set; }
    public DbSet<Arkadaslik> Arkadasliklar { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-KK050KP\\SQLEXPRESS;Database=KadinUrun;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adminler>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Adminler__719FE4E8E0366B44");

            entity.ToTable("Adminler");

            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.KullaniciAdi).HasMaxLength(50);
            entity.Property(e => e.Sifre).HasMaxLength(100);
        });

        modelBuilder.Entity<Kategoriler>(entity =>
        {
            entity.HasKey(e => e.KategoriId).HasName("PK__Kategori__1782CC92B2168735");

            entity.ToTable("Kategoriler");

            entity.Property(e => e.KategoriId).HasColumnName("KategoriID");
            entity.Property(e => e.KategoriAdi).HasMaxLength(100);
        });

        modelBuilder.Entity<Kullanicilar>(entity =>
        {
            entity.HasKey(e => e.KullaniciId).HasName("PK__Kullanic__E011F09B982A6ED3");

            entity.ToTable("Kullanicilar");

            entity.HasIndex(e => e.Email, "UQ__Kullanic__A9D105346CD89D1C").IsUnique();

            entity.Property(e => e.KullaniciId).HasColumnName("KullaniciID");
            entity.Property(e => e.AdSoyad).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsAdmin).HasDefaultValue(false);
            entity.Property(e => e.KayitTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProfilFotoUrl)
                .HasMaxLength(300)
                .HasColumnName("ProfilFotoURL");
            entity.Property(e => e.Sifre).HasMaxLength(100);
        });

        modelBuilder.Entity<Puanlar>(entity =>
        {
            entity.HasKey(e => e.PuanId).HasName("PK__Puanlar__04A64C6E925C743A");

            entity.ToTable("Puanlar");

            entity.Property(e => e.PuanId).HasColumnName("PuanID");
            entity.Property(e => e.KullaniciId).HasColumnName("KullaniciID");
            entity.Property(e => e.UrunId).HasColumnName("UrunID");

            entity.HasOne(d => d.Kullanici).WithMany(p => p.Puanlars)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Puanlar__Kullani__4AB81AF0");

            entity.HasOne(d => d.Urun).WithMany(p => p.Puanlars)
                .HasForeignKey(d => d.UrunId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Puanlar__UrunID__49C3F6B7");
        });

        modelBuilder.Entity<Urunler>(entity =>
        {
            entity.HasKey(e => e.UrunId).HasName("PK__Urunler__623D364B8D52E8D8");

            entity.ToTable("Urunler");

            entity.Property(e => e.UrunId).HasColumnName("UrunID");
            entity.Property(e => e.EklenmeTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EkleyenKullaniciId).HasColumnName("EkleyenKullaniciID");
            entity.Property(e => e.KategoriId).HasColumnName("KategoriID");
            entity.Property(e => e.ResimUrl)
                .HasMaxLength(300)
                .HasColumnName("ResimURL");
            entity.Property(e => e.UrunAdi).HasMaxLength(200);

            entity.HasOne(d => d.EkleyenKullanici).WithMany(p => p.Urunlers)
                .HasForeignKey(d => d.EkleyenKullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Urunler__Ekleyen__412EB0B6");

            entity.HasOne(d => d.Kategori).WithMany(p => p.Urunlers)
                .HasForeignKey(d => d.KategoriId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Urunler__Kategor__403A8C7D");
        });

        modelBuilder.Entity<Yorumlar>(entity =>
        {
            entity.HasKey(e => e.YorumId).HasName("PK__Yorumlar__F2BE14C8ECB0227D");

            entity.ToTable("Yorumlar");

            entity.Property(e => e.YorumId).HasColumnName("YorumID");
            entity.Property(e => e.KullaniciId).HasColumnName("KullaniciID");
            entity.Property(e => e.UrunId).HasColumnName("UrunID");
            entity.Property(e => e.YorumFotoUrl)
                .HasMaxLength(300)
                .HasColumnName("YorumFotoURL");
            entity.Property(e => e.YorumTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Kullanici).WithMany(p => p.Yorumlars)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Yorumlar__Kullan__45F365D3");

            entity.HasOne(d => d.Urun).WithMany(p => p.Yorumlars)
                .HasForeignKey(d => d.UrunId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Yorumlar__UrunID__44FF419A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
