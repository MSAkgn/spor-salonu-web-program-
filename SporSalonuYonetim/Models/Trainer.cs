using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        
        [Required]
        public string AdSoyad { get; set; }
        
        public string UzmanlikAlani { get; set; } // Örn: Kilo Verme, Kas Geliştirme
        
        // Antrenörün verebildiği hizmetler
        public ICollection<Service>? Services { get; set; }

        // Basit müsaitlik kontrolü için (Detaylısı Appointment ile yapılır)
        public TimeSpan CalismaBaslangicSaati { get; set; } // 09:00
        public TimeSpan CalismaBitisSaati { get; set; }    // 18:00
    }
}