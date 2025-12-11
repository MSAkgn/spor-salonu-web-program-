using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Data;
using SporSalonuYonetim.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection; // <-- BU KÜTÜPHANE ŞART

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı Bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Identity Ayarları (Senin özel şifre kuralların)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3; 
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// --- İŞTE İSTEDİĞİN ÖZELLİK BURASI ---
// Program durdurulduğu an, şifre çözme anahtarlarını yok et.
// Program tekrar başladığında eski çerezler "okunamayan çöp" olur ve giriş düşer.
builder.Services.AddDataProtection()
    .UseEphemeralDataProtectionProvider(); 
// -------------------------------------

// Ayrıca tarayıcı kapandığında da çerezi silsin (Session Cookie)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // 60 dk ömür
    options.SlidingExpiration = true; // Hareket ettikçe süre uzasın
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    
    // Tarayıcı kapatılınca cookie silinsin mi? EVET.
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); 

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication SIRASI ÖNEMLİ
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// DbSeeder (Admin Oluşturma)
using (var scope = app.Services.CreateScope())
{
    // Program her başladığında Admin'in güvenlik mührünü de değiştiriyoruz (Çifte Garanti)
    await DbSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);
}

app.Run();