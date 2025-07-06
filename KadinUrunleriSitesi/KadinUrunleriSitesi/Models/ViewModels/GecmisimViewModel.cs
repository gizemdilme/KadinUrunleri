using System;
using System.Collections.Generic;
using KadinUrunleriSitesi.Models;

namespace KadinUrunleriSitesi.Models.ViewModels
{
    public class GecmisimViewModel
    {
        public List<Yorumlar> Yorumlar { get; set; } = new List<Yorumlar>();
        public List<Puanlar> Puanlar { get; set; } = new List<Puanlar>();
    }
}
