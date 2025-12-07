using Microsoft.AspNetCore.Identity;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            // Kullanıcı ve Rol yöneticilerini çağır
            var userManager = service.GetService<UserManager<ApplicationUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            // 1. Rolleri Kontrol Et ve Yoksa Oluştur
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("Member"))
                await roleManager.CreateAsync(new IdentityRole("Member"));

            // 2. Admin Kullanıcısını Oluştur
            // BURAYI KENDİ NUMARANIZLA GÜNCELLEYİN 👇
            var adminEmail = "b231210032@sakarya.edu.tr"; 
            
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

                // Şifre: sau (Ödevde istenen şifre)
                var result = await userManager.CreateAsync(newAdmin, "sau");
                
                if (result.Succeeded)
                {
                    // Kullanıcıya Admin rolünü ver
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}