using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers.Api
{
    // Bu bir API Controller'dır (View döndürmez, JSON veri döndürür)
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Tüm Antrenörleri Getiren Endpoint
        // Örnek Çağrı: GET /api/reportsapi/trainers
        [HttpGet("trainers")]
        public async Task<ActionResult<IEnumerable<object>>> GetTrainers()
        {
            // LINQ Sorgusu: Sadece gerekli alanları seçiyoruz (Select)
            var trainers = await _context.Trainers
                .Select(t => new { 
                    t.AdSoyad, 
                    t.UzmanlikAlani 
                })
                .ToListAsync();

            return Ok(trainers);
        }

        // 2. Tarihe Göre Randevuları Getiren (Filtreleme) Endpoint
        // Örnek Çağrı: GET /api/reportsapi/appointments?date=2023-12-12
        [HttpGet("appointments")]
        public async Task<ActionResult<IEnumerable<object>>> GetAppointmentsByDate(DateTime? date)
        {
            // LINQ Filtreleme: Eğer tarih girilmemişse bugünü al
            var filterDate = date ?? DateTime.Today;

            var appointments = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.User)
                // LINQ Where Sorgusu (Tarih filtreleme)
                .Where(a => a.TarihSaat.Date == filterDate.Date)
                .Select(a => new {
                    Tarih = a.TarihSaat.ToString("HH:mm"),
                    Antrenor = a.Trainer.AdSoyad,
                    Uye = a.User.Email,
                    Durum = a.OnaylandiMi ? "Onaylı" : "Bekliyor"
                })
                .ToListAsync();

            return Ok(appointments);
        }
    }
}