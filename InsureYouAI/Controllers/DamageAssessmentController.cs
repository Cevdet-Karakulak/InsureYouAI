using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAINew.Controllers
{
    public class DamageAssessmentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _claudeApiKey;

        public DamageAssessmentController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            // Mevcut Claude anahtarını kullanıyoruz
            _claudeApiKey = configuration["ClaudeAI:ApiKey"];
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.ControllerName = "Hasar Tespit Modülü";
            ViewBag.PageName = "AI Görsel Analiz";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeDamage(IFormFile imageFile)
        {
            ViewBag.ControllerName = "Hasar Tespit Modülü";
            ViewBag.PageName = "AI Görsel Analiz";

            if (imageFile == null || imageFile.Length == 0)
            {
                ViewBag.Error = "Lütfen analiz edilecek bir hasar fotoğrafı yükleyin.";
                return View("Index");
            }

            // Sadece görsel dosyalarına izin ver
            if (!imageFile.ContentType.StartsWith("image/"))
            {
                ViewBag.Error = "Lütfen geçerli bir resim dosyası (JPEG, PNG vb.) yükleyin.";
                return View("Index");
            }

            try
            {
                // 1. Görseli Base64 formatına çevir
                string base64Image = await ConvertFileToBase64Async(imageFile);
                string mediaType = imageFile.ContentType;

                // 2. Claude Vision API'ye gönder ve sonucu al
                string analysisResult = await CallClaudeVisionApi(base64Image, mediaType);

                // Görseli ekranda göstermek için View'a geri gönderiyoruz
                ViewBag.ImageBase64 = $"data:{mediaType};base64,{base64Image}";
                ViewBag.AnalysisResult = analysisResult;
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Analiz sırasında bir hata oluştu: {ex.Message}";
            }

            return View("Index");
        }

        // Yardımcı Metot: Dosyayı Base64 string'e çevirir
        private async Task<string> ConvertFileToBase64Async(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            return Convert.ToBase64String(fileBytes);
        }

        // Ana Metot: Claude Vision API Çağrısı
        private async Task<string> CallClaudeVisionApi(string base64Image, string mediaType)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");

            request.Headers.Add("x-api-key", _claudeApiKey);
            request.Headers.Add("anthropic-version", "2023-06-01");

            // Sigorta eksperi gibi düşünmesi için özel prompt
            var promptText = @"
Sen uzman bir sigorta eksperisin. Görevin bu fotoğraftaki hasarı analiz etmek.
Lütfen aşağıdaki formatta, Türkçe bir rapor hazırla:

1. Hasar Türü: (Örn: Çarpışma, su baskını, yangın, dolu hasarı vb.)
2. Etkilenen Parçalar/Alanlar: (Gördüğün tüm hasarlı bölgeleri listele)
3. Hasar Şiddeti: (Hafif / Orta / Ağır - Nedenini kısaca açıkla)
4. Tahmini Onarım Süreci: (Değişim mi gerekli, onarım mı?)
5. Eksper Notu: (Sigorta şirketi için kritik bir uyarı veya gözlem)

Yanıtı sadece bu başlıklar altında maddeler halinde ver.
";

            // Claude Vision API için özel body yapısı
            var requestBody = new
            {
                model = "claude-3-7-sonnet-20250219", // Hata veren eski ismi bununla değiştirdik
                max_tokens = 2048,
                temperature = 0.2,
                messages = new[]
        {
            new
            {
                role = "user",
                content = new object[]
                {
                    new { type = "text", text = promptText },
                    new
                    {
                        type = "image",
                        source = new
                        {
                            type = "base64",
                            media_type = mediaType,
                            data = base64Image
                        }
                    }
                }
            }
        }
            };

            var json = JsonSerializer.Serialize(requestBody);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - {responseText}");
            }

            using var doc = JsonDocument.Parse(responseText);
            return doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString();
        }
    }
}