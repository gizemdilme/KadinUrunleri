namespace KadinUrunleriSitesi.Models
{
    public class Arkadaslik
    {
        public int Id { get; set; }
        public int GonderenId { get; set; }
        public int AlanId { get; set; }
        public DateTime Tarih { get; set; }
        public string Durum { get; set; }

        public Kullanicilar Gonderen { get; set; }
        public Kullanicilar Alan { get; set; }
    }
}
