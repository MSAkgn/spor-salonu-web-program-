namespace SporSalonuYonetim.Models
{
    public class AiViewModel
    {
        // Kullanıcıdan alacaklarımız
        public double Boy { get; set; } // Santimetre cinsinden (Örn: 180)
        public double Kilo { get; set; } // Kg cinsinden (Örn: 75)

        // Bizim hesaplayıp göndereceklerimiz
        public double? BmiSonuc { get; set; }
        public string? Durum { get; set; }     // Örn: Normal, Kilolu
        public string? Renk { get; set; }      // Örn: success, warning, danger
        public string? Oneri { get; set; }     // AI Tavsiyesi
    }
}