using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Controllers
{
    [Authorize] // Sadece üyeler kullanabilsin
    public class AiController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new AiViewModel());
        }

        [HttpPost]
        public IActionResult Index(AiViewModel model)
        {
            if (model.Boy > 0 && model.Kilo > 0)
            {
                // BMI Formülü: Kilo / (Boy(metre) * Boy(metre))
                double boyMetre = model.Boy / 100.0;
                double bmi = model.Kilo / (boyMetre * boyMetre);

                model.BmiSonuc = Math.Round(bmi, 2);

                // --- YAPAY ZEKA MANTIĞI (Kural Tabanlı) ---
                if (bmi < 18.5)
                {
                    model.Durum = "Zayıf";
                    model.Renk = "info"; // Mavi
                    model.Oneri = "🤖 AI Önerisi: Kalori alımını artırmalısın! " +
                                  "Hacim kazanmak (Bulking) için karbonhidrat ağırlıklı beslen " +
                                  "ve ağır ağırlıklarla az tekrar (Hypertrophy) çalış.";
                }
                else if (bmi >= 18.5 && bmi < 25)
                {
                    model.Durum = "İdeal Kilo";
                    model.Renk = "success"; // Yeşil
                    model.Oneri = "🤖 AI Önerisi: Harikasın! Formunu korumak için dengeli beslenmeye devam et. " +
                                  "Haftada 3 gün tüm vücut (Full Body) antrenmanı senin için ideal.";
                }
                else if (bmi >= 25 && bmi < 30)
                {
                    model.Durum = "Fazla Kilolu";
                    model.Renk = "warning"; // Sarı
                    model.Oneri = "🤖 AI Önerisi: Hafif bir kalori açığı oluşturmalısın. " +
                                  "Antrenmanlarına 20 dakika HIIT kardiyo ekle ve şekerden uzak dur. " +
                                  "Yağ yakımına odaklan.";
                }
                else
                {
                    model.Durum = "Obezite Sınırı";
                    model.Renk = "danger"; // Kırmızı
                    model.Oneri = "🤖 AI Önerisi: Sağlığın için harekete geçme zamanı! " +
                                  "Düşük tempolu kardiyo (Yürüyüş, Bisiklet) ile başla. " +
                                  "Mutlaka uzman diyetisyen ve antrenörlerimizden birebir destek al.";
                }
            }
            else
            {
                ModelState.AddModelError("", "Lütfen geçerli değerler giriniz.");
            }

            return View(model);
        }
    }
}