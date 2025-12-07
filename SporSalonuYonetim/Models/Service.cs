using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        public string Ad { get; set; } // Örn: Yoga, Pilates

        public string Aciklama { get; set; }
        
        public int SureDakika { get; set; } // Örn: 60 dk
        
        public decimal Ucret { get; set; } // Örn: 500 TL
        
        // Bir hizmeti birden fazla antrenör verebilir
        public ICollection<Trainer>? Trainers { get; set; }
    }
}