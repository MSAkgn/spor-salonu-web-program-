using Microsoft.AspNetCore.Http; // Bu satır IFormFile için zorunludur

namespace SporSalonuYonetim.Models
{
    public class AiViewModel
    {
        // Girdiler
        public double Boy { get; set; }
        public double Kilo { get; set; }
        
        // Hata veren eksik kısımlar bunlardı:
        public IFormFile? Foto { get; set; } // Dosya yüklemek için
        
        // Çıktılar
        public double? BmiSonuc { get; set; }
        public string? Durum { get; set; }
        public string? Renk { get; set; }
        public string? Oneri { get; set; }
        
        // Ekranda resmi göstermek için eksik olan diğer kısım:
        public string? FotoDataUrl { get; set; } 
    }
}