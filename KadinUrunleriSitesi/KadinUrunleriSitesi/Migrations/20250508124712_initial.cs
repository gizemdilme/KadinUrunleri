using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KadinUrunleriSitesi.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adminler",
                columns: table => new
                {
                    AdminID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Adminler__719FE4E8E0366B44", x => x.AdminID);
                });

            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    KategoriID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KategoriAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Kategori__1782CC92B2168735", x => x.KategoriID);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    KullaniciID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    ProfilFotoURL = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Kullanic__E011F09B982A6ED3", x => x.KullaniciID);
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    UrunID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResimURL = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    KategoriID = table.Column<int>(type: "int", nullable: false),
                    EkleyenKullaniciID = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Urunler__623D364B8D52E8D8", x => x.UrunID);
                    table.ForeignKey(
                        name: "FK__Urunler__Ekleyen__412EB0B6",
                        column: x => x.EkleyenKullaniciID,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciID");
                    table.ForeignKey(
                        name: "FK__Urunler__Kategor__403A8C7D",
                        column: x => x.KategoriID,
                        principalTable: "Kategoriler",
                        principalColumn: "KategoriID");
                });

            migrationBuilder.CreateTable(
                name: "Puanlar",
                columns: table => new
                {
                    PuanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunID = table.Column<int>(type: "int", nullable: false),
                    KullaniciID = table.Column<int>(type: "int", nullable: false),
                    Puan = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Puanlar__04A64C6E925C743A", x => x.PuanID);
                    table.ForeignKey(
                        name: "FK__Puanlar__Kullani__4AB81AF0",
                        column: x => x.KullaniciID,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciID");
                    table.ForeignKey(
                        name: "FK__Puanlar__UrunID__49C3F6B7",
                        column: x => x.UrunID,
                        principalTable: "Urunler",
                        principalColumn: "UrunID");
                });

            migrationBuilder.CreateTable(
                name: "Yorumlar",
                columns: table => new
                {
                    YorumID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunID = table.Column<int>(type: "int", nullable: false),
                    KullaniciID = table.Column<int>(type: "int", nullable: false),
                    Icerik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YorumTarihi = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    YorumFotoURL = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Yorumlar__F2BE14C8ECB0227D", x => x.YorumID);
                    table.ForeignKey(
                        name: "FK__Yorumlar__Kullan__45F365D3",
                        column: x => x.KullaniciID,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciID");
                    table.ForeignKey(
                        name: "FK__Yorumlar__UrunID__44FF419A",
                        column: x => x.UrunID,
                        principalTable: "Urunler",
                        principalColumn: "UrunID");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__Kullanic__A9D105346CD89D1C",
                table: "Kullanicilar",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Puanlar_KullaniciID",
                table: "Puanlar",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_Puanlar_UrunID",
                table: "Puanlar",
                column: "UrunID");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_EkleyenKullaniciID",
                table: "Urunler",
                column: "EkleyenKullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_KategoriID",
                table: "Urunler",
                column: "KategoriID");

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_KullaniciID",
                table: "Yorumlar",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_Yorumlar_UrunID",
                table: "Yorumlar",
                column: "UrunID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adminler");

            migrationBuilder.DropTable(
                name: "Puanlar");

            migrationBuilder.DropTable(
                name: "Yorumlar");

            migrationBuilder.DropTable(
                name: "Urunler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Kategoriler");
        }
    }
}
