using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SporSalonuYonetim.Models;
using System.Text;
using System.Text.Json; 

namespace SporSalonuYonetim.Controllers
{
    [Authorize]
    public class AiController : Controller
    {
        // 🔴 LİSTEYİ ALABİLDİĞİN O "ÇALIŞAN" API KEY'İ BURAYA YAPIŞTIR
        private const string GeminiApiKey = "AIzaSyBfvliBf4yBuIZoJX3jiz9DU3lxaCBZUmA"; 
        
        // ✨ HEDEF: Listedeki en yeni ve güçlü model: Gemini 2.5 Flash
        // Google senin hesabına bu sürümü tanımlamış.
        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        [HttpGet]
        public IActionResult Index()
        {
            return View(new AiViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(AiViewModel model)
        {
            // Giriş Kontrolleri
            bool veriGirildi = (model.Boy > 0 && model.Kilo > 0);
            bool fotoGirildi = (model.Foto != null && model.Foto.Length > 0);

            if (!veriGirildi && !fotoGirildi)
            {
                model.Oneri = "Lütfen analiz için Boy/Kilo girin veya bir Fotoğraf yükleyin.";
                model.Renk = "danger";
                return View(model);
            }

            // BMI Hesaplama
            if (veriGirildi)
            {
                double boyMetre = model.Boy / 100.0;
                double bmi = model.Kilo / (boyMetre * boyMetre);
                model.BmiSonuc = Math.Round(bmi, 2);
                
                model.Durum = (bmi < 25) ? "Normal" : "Dikkat";
                model.Renk = (bmi < 25) ? "success" : "warning";
            }

            try 
            {
                string base64Image = null;
                string contentType = null;

                // Fotoğraf varsa işle
                if (fotoGirildi)
                {
                    using (var ms = new MemoryStream())
                    {
                        await model.Foto.CopyToAsync(ms);
                        base64Image = Convert.ToBase64String(ms.ToArray());
                        contentType = model.Foto.ContentType;
                        model.FotoDataUrl = $"data:{contentType};base64,{base64Image}";
                    }
                }

                // AI Çağır
                model.Oneri = await CallGeminiAi(model.Boy, model.Kilo, base64Image, contentType);
            }
            catch (Exception ex)
            {
                model.Oneri = $"Sistem Hatası: {ex.Message}";
            }

            return View(model);
        }

        private async Task<string> CallGeminiAi(double boy, double kilo, string? base64Image, string? mimeType)
        {
            using (var client = new HttpClient())
            {
                // Prompt
                string prompt = "Sen uzman bir spor koçusun. ";

                if (boy > 0 && kilo > 0)
                {
                    prompt += $"Boyum: {boy} cm, Kilom: {kilo} kg. ";
                    if(string.IsNullOrEmpty(base64Image))
                    {
                         prompt += "Fotoğraf yok, sadece verilere göre BMI yorumu, beslenme ve antrenman önerisi ver. ";
                    }
                }

                if (!string.IsNullOrEmpty(base64Image))
                {
                    prompt += "Yüklediğim fotoğrafı analiz et, tahmini yağ oranımı söyle. ";
                }

                prompt += "Lütfen cevabı HTML formatında (<b>, <br>, <ul>) maddeler halinde, motive edici bir dille yaz. <html> etiketi kullanma.";

                // JSON Paketleme
                var partsList = new List<object>();
                partsList.Add(new { text = prompt });

                if (!string.IsNullOrEmpty(base64Image))
                {
                    partsList.Add(new 
                    { 
                        inline_data = new 
                        { 
                            mime_type = mimeType, 
                            data = base64Image
                        } 
                    });
                }

                var requestBody = new
                {
                    contents = new object[]
                    {
                        new { parts = partsList }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // İstek Gönder
                var response = await client.PostAsync($"{ApiUrl}?key={GeminiApiKey}", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(responseString))
                    {
                        if(doc.RootElement.TryGetProperty("candidates", out JsonElement candidates) && candidates.GetArrayLength() > 0)
                        {
                            return candidates[0]
                                .GetProperty("content")
                                .GetProperty("parts")[0]
                                .GetProperty("text")
                                .GetString();
                        }
                        return "Yapay zeka analiz yaptı ama cevap boş döndü.";
                    }
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    
                    // Kota hatası (429) gelirse B PLANI için ipucu verelim
                    if((int)response.StatusCode == 429)
                        return "Hata: Kota doldu. Kodun içindeki ApiUrl kısmını 'gemini-2.0-flash-lite-001' olarak değiştirip deneyin.";

                    return $"API Hatası ({response.StatusCode}): {errorMsg}";
                }
            }
        }
    }
}