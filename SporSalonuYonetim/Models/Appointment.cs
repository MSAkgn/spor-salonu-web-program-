using System.ComponentModel.DataAnnotations;

namespace SporSalonuYonetim.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; } // Randevuyu alan üye

        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; } // Seçilen antrenör

        public int ServiceId { get; set; }
        public Service Service { get; set; } // Seçilen hizmet

        [Required]
        public DateTime TarihSaat { get; set; }

        public bool OnaylandiMi { get; set; } = false; // Admin onayı
    }
}