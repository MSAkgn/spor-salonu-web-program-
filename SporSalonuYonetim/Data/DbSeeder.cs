using Microsoft.AspNetCore.Identity;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<ApplicationUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            // 1. Rolleri Kontrol Et
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("Member"))
                await roleManager.CreateAsync(new IdentityRole("Member"));

            // 2. Admin Kullanıcısını Yönet
            var adminEmail = "g211210001@sakarya.edu.tr"; // Numaran buradaydı
            
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    AdSoyad = "Sistem Yöneticisi",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, "sau");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
            else 
            {
                // --- YENİ EKLENEN KISIM ---
                // Eğer Admin zaten varsa, Güvenlik Mührünü değiştir.
                // Bu işlem, eski oturumların (Cookie'lerin) anında geçersiz olmasını sağlar.
                await userManager.UpdateSecurityStampAsync(adminUser);
            }
        }
    }
}