using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// AŞAĞIDAKİ SATIR ÇOK ÖNEMLİ, HATAYI BU ÇÖZECEK:
using SporSalonuYonetim.Models; 

namespace SporSalonuYonetim.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Trainer>()
                .HasMany(t => t.Services)
                .WithMany(s => s.Trainers)
                .UsingEntity(j => j.ToTable("TrainerServices"));
        }
    }
}