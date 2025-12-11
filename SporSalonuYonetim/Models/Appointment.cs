using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lütfen bir tarih seçiniz.")]
        public DateTime TarihSaat { get; set; }

        public bool OnaylandiMi { get; set; } = false;

        // --- İLİŞKİLER (Soru İşaretlerine Dikkat) ---

        public string? UserId { get; set; } // ? Eklendi
        public ApplicationUser? User { get; set; } // ? Eklendi

        [Required(ErrorMessage = "Antrenör seçimi zorunludur.")]
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; } // ? Eklendi

        [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
        public int ServiceId { get; set; }
        public Service? Service { get; set; } // ? Eklendi
    }
}