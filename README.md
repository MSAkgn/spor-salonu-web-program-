# spor-salonu-web-program-#
Spor Salonu Yönetim ve Randevu Sistemi (Fitness Center Management System)

Bu proje, Sakarya Üniversitesi **Web Programlama** dersi 2025-2026 Güz Dönemi proje ödevi kapsamında geliştirilmiştir. Projenin temel amacı, bir spor salonunun yönetim süreçlerini dijitalleştirmek, üye randevularını organize etmek ve yapay zeka desteği ile kişiselleştirilmiş deneyim sunmaktır.

## 🎯 Projenin Amacı ve Kapsamı
Bu proje, spor salonları için antrenör yönetimi, hizmet tanımlama ve üye randevu takibi gibi temel işlevleri yerine getiren bir web uygulamasıdır. Ayrıca, yapay zeka entegrasyonu sayesinde üyelere kişisel egzersiz ve diyet önerileri sunmayı hedefler.

## 🚀 Özellikler

### 1. Kullanıcı ve Rol Yönetimi
* **Admin ve Üye Panelleri:** Rol bazlı yetkilendirme (Admin, Üye).
* **Kayıt/Giriş:** Güvenli kullanıcı doğrulama ve kayıt işlemleri.

### 2. Salon ve Antrenör Yönetimi
* **Hizmet Tanımları:** Fitness, Yoga, Pilates vb. hizmetlerin süre ve ücret bilgileriyle tanımlanması.
* **Antrenör Profilleri:** Antrenörlerin uzmanlık alanları ve çalışma saatlerinin (müsaitlik) yönetimi.

### 3. Randevu Sistemi
* **Online Randevu:** Üyelerin uygun antrenör ve saat aralığını seçerek randevu alabilmesi.
* **Çakışma Kontrolü:** Sistemin dolu saatleri otomatik engelleyerek çakışmayı önlemesi.
* **Onay Mekanizması:** Randevuların sistem tarafından yönetilmesi ve onaylanması.

### 4. REST API ve Raporlama
* Veritabanı ile iletişim kuran REST API entegrasyonu.
* **LINQ Sorguları:** Antrenör listeleme, tarih bazlı filtreleme ve üye randevu geçmişi gibi verilerin API üzerinden filtrelenerek sunulması.

### 5. Yapay Zeka (AI) Entegrasyonu
* Kullanıcı verilerine (boy, kilo, hedef veya fotoğraf) dayalı olarak yapay zeka destekli egzersiz veya diyet programı önerisi.

## 🛠️ Kullanılan Teknolojiler
* **Platform:** ASP.NET Core MVC (LTS)
* **Dil:** C#
* **Veritabanı:** SQL Server / PostgreSQL
* **ORM:** Entity Framework Core (Code First & LINQ)
* **Frontend:** HTML5, CSS3, JavaScript, jQuery
* **UI Framework:** Bootstrap 5
* **AI Servisi:** OpenAI API / Custom Model

## 🔐 Varsayılan Admin Bilgileri
Proje değerlendirmesi için oluşturulan varsayılan yönetici hesabı:

* **Email:** `ogrencinumarasi@sakarya.edu.tr`
* **Şifre:** `sau`

## ⚙️ Kurulum (Installation)

1.  Projeyi klonlayın:
    ```bash
    git clone [REPO_LINKINIZ_BURAYA]
    ```
2.  `appsettings.json` dosyasındaki veritabanı bağlantı dizesini (Connection String) kendi sunucunuza göre düzenleyin.
3.  Package Manager Console üzerinden veritabanını oluşturun:
    ```bash
    Update-Database
    ```
4.  Projeyi çalıştırın.

## 👥 Proje Ekibi
* **Ad Soyad: Muhammet Süleyman AKGÜN
* **Öğrenci No: B231210032
* **Ders Grubu: 1.Öğretim C Grubu

---
*Not: Bu proje Web Programlama dersi gereksinimlerine uygun olarak akademik amaçla geliştirilmiştir.*
