using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            // 1. KONTROL: Kullanıcı giriş yapmış mı?
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Randevularınızı görebilmek için önce giriş yapmalısınız.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var randevular = _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .Include(a => a.User)
                .AsQueryable();

            if (!isAdmin)
            {
                randevular = randevular.Where(a => a.UserId == user.Id);
            }

            return View(await randevular.OrderBy(a => a.TarihSaat).ToListAsync());
        }

        [Authorize] 
        public IActionResult Create()
        {
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "AdSoyad");
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Ad");
            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] 
        public async Task<IActionResult> Create([Bind("Id,TrainerId,ServiceId,TarihSaat")] Appointment appointment)
        {
            // 1. Kullanıcı Atama
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });
            appointment.UserId = user.Id;

            // 2. Geçmiş Tarih Kontrolü
            if (appointment.TarihSaat < DateTime.Now)
            {
                ModelState.AddModelError("", "Geçmiş bir tarihe randevu alamazsınız.");
            }

            // 3. ÇAKIŞMA KONTROLÜ (Başka randevu var mı?)
            bool cakismaVarMi = _context.Appointments.Any(a => 
                a.TrainerId == appointment.TrainerId && 
                a.TarihSaat == appointment.TarihSaat);

            if (cakismaVarMi)
            {
                ModelState.AddModelError("", "Seçtiğiniz antrenör bu saatte dolu! Lütfen başka bir saat seçiniz.");
            }

            // --- YENİ EKLENEN KISIM BAŞLANGIÇ ---
            // 4. MESAİ SAATİ KONTROLÜ (Antrenör çalışıyor mu?)
            // Seçilen antrenörü veritabanından çekip saatlerine bakıyoruz
            var secilenAntrenor = await _context.Trainers.FindAsync(appointment.TrainerId);
            
            if (secilenAntrenor != null)
            {
                // Randevunun saati (Sadece saat kısmı, tarih önemsiz)
                TimeSpan randevuSaati = appointment.TarihSaat.TimeOfDay;

                // Eğer randevu saati, başlangıçtan önceyse VEYA bitişten sonraysa hata ver
                if (randevuSaati < secilenAntrenor.CalismaBaslangicSaati || randevuSaati >= secilenAntrenor.CalismaBitisSaati)
                {
                    ModelState.AddModelError("", $"Seçtiğiniz antrenör o saatte çalışmıyor. (Çalışma Saatleri: {secilenAntrenor.CalismaBaslangicSaati} - {secilenAntrenor.CalismaBitisSaati})");
                }
            }
            // --- YENİ EKLENEN KISIM BİTİŞ ---

            // Validation temizliği
            ModelState.Remove("User");
            ModelState.Remove("UserId");
            ModelState.Remove("Trainer");
            ModelState.Remove("Service");

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "AdSoyad", appointment.TrainerId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Ad", appointment.ServiceId);
            return View(appointment);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var randevu = await _context.Appointments.FindAsync(id);
            if (randevu != null)
            {
                randevu.OnaylandiMi = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}