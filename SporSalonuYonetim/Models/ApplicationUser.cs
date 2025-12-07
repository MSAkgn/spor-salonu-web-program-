using Microsoft.AspNetCore.Identity;

namespace SporSalonuYonetim.Models
{
    // Standart IdentityUser'dan miras alıp ekstra özellikler ekliyoruz
    public class ApplicationUser : IdentityUser
    {
        public string AdSoyad { get; set; }
        public int? Yas { get; set; }
        public double? Boy { get; set; } // AI hesaplaması için
        public double? Kilo { get; set; } // AI hesaplaması için
        public string? Cinsiyet { get; set; }
    }
}