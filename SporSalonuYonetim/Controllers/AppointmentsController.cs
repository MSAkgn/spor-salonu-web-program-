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
            if (!User.Identity.IsAuthenticated)
            {
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

        // GET: Appointments/Create
        [Authorize]
        public IActionResult Create(int? trainerId, int? serviceId)
        {
            // Hizmetleri Dropdown için hazırlıyoruz
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Ad", serviceId);

            // Antrenörleri Dropdown için 'Liste' olarak çekiyoruz (View tarafında uzmanlık alanına erişmek için)
            ViewBag.Trainers = _context.Trainers.ToList();

            // Linkten gelen seçimleri View'a taşıyoruz
            ViewBag.SelectedTrainerId = trainerId;
            ViewBag.SelectedServiceId = serviceId;

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
            
            appointment.UserId = user.Id; // Otomatik atama

            // 2. Geçmiş Tarih Kontrolü
            if (appointment.TarihSaat < DateTime.Now)
            {
                ModelState.AddModelError("", "Geçmiş bir tarihe randevu alamazsınız.");
            }

            // 3. ÇAKIŞMA KONTROLÜ
            bool cakismaVarMi = _context.Appointments.Any(a => 
                a.TrainerId == appointment.TrainerId && 
                a.TarihSaat == appointment.TarihSaat);

            if (cakismaVarMi)
            {
                ModelState.AddModelError("", "Seçtiğiniz antrenör bu saatte dolu! Lütfen başka bir saat seçiniz.");
            }

            // 4. MESAİ SAATİ KONTROLÜ
            var secilenAntrenor = await _context.Trainers.FindAsync(appointment.TrainerId);
            
            if (secilenAntrenor != null)
            {
                TimeSpan randevuSaati = appointment.TarihSaat.TimeOfDay;
                if (randevuSaati < secilenAntrenor.CalismaBaslangicSaati || randevuSaati >= secilenAntrenor.CalismaBitisSaati)
                {
                    ModelState.AddModelError("", $"Seçtiğiniz antrenör o saatte çalışmıyor. ({secilenAntrenor.CalismaBaslangicSaati} - {secilenAntrenor.CalismaBitisSaati})");
                }
            }

            // Validation Temizliği
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

            // Hata varsa sayfayı tekrar doldur
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Ad", appointment.ServiceId);
            ViewBag.Trainers = _context.Trainers.ToList(); // Burayı unutmamak önemli
            ViewBag.SelectedTrainerId = appointment.TrainerId;
            
            return View(appointment);
        }

        // ... Diğer metodlar (Approve, Delete vs) aynı kalabilir ...
        
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
            if (appointment != null) _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}