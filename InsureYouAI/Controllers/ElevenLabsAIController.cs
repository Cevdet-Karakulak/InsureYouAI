using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.Extensions.Configuration; // IConfiguration için gerekli

namespace InsureYouAINew.Controllers
{
    public class ElevenLabsAIController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _elevenLabsKey;

        // Constructor üzerinden IConfiguration ve IHttpClientFactory enjekte edildi
        public ElevenLabsAIController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            // secrets.json içindeki ElevenLabs:ApiKey değerini okur
            _elevenLabsKey = configuration["ElevenLabs:ApiKey"];
        }

        #region SpeakInsuranceAnswer (Versiyon 1)
        public IActionResult SpeakInsuranceAnswer()
        {
            ViewBag.ControllerName = "Eleven Labs";
            ViewBag.PageName = "Sesli Yapay Zeka Asistanı";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SpeakInsuranceAnswer(string text)
        {
            ViewBag.ControllerName = "Eleven Labs";
            ViewBag.PageName = "Sesli Yapay Zeka Asistanı";

            if (string.IsNullOrWhiteSpace(text))
            {
                ViewBag.Error = "Lütfen metin gir.";
                return View();
            }

            var voiceId = "EXAVITQu4vr4xnSDxMaL";
            var url = $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}/stream";

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("xi-api-key", _elevenLabsKey); // Config'den gelen anahtar

            var requestBody = new
            {
                text = text,
                model_id = "eleven_multilingual_v2",
                voice_settings = new { stability = 0.5, similarity_boost = 0.8 }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Ses oluşturulamadı. API anahtarı veya bakiye yetersiz olabilir.";
                return View();
            }

            return await SaveAndReturnAudio(response);
        }
        #endregion

        #region SpeakInsuranceAnswer2 (Versiyon 2)
        public IActionResult SpeakInsuranceAnswer2()
        {
            ViewBag.ControllerName = "Eleven Labs";
            ViewBag.PageName = "Sesli Yapay Zeka Asistanı";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SpeakInsuranceAnswer2(string text)
        {
            ViewBag.ControllerName = "Eleven Labs";
            ViewBag.PageName = "Sesli Yapay Zeka Asistanı";

            if (string.IsNullOrWhiteSpace(text))
            {
                ViewBag.Error = "Lütfen metin gir.";
                return View();
            }

            var voiceId = "EXAVITQu4vr4xnSDxMaL";
            var url = $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}/stream";

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("xi-api-key", _elevenLabsKey); // Config'den gelen anahtar

            var requestBody = new
            {
                text = text,
                model_id = "eleven_multilingual_v2",
                voice_settings = new { stability = 0.5, similarity_boost = 0.8 }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Ses oluşturulamadı.";
                return View();
            }

            return await SaveAndReturnAudio(response);
        }
        #endregion

        #region SpeakInsuranceAnswer3 (Versiyon 3 - Sohbet Görünümlü)
        public IActionResult SpeakInsuranceAnswer3()
        {
            ViewBag.ControllerName = "Eleven Labs";
            ViewBag.PageName = "Sesli Yapay Zeka Asistanı";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SpeakInsuranceAnswer3(string text)
        {
            ViewBag.ControllerName = "Yapay Zeka Sigorta Asistanı";
            ViewBag.PageName = "Sesli Yanıt & Akıllı Sohbet Alanı";

            if (string.IsNullOrWhiteSpace(text))
            {
                ViewBag.Answer = "Lütfen bir metin girin.";
                return View();
            }

            string aiTextResponse = $"InsureYOU AI yanıtı: {text}";
            ViewBag.Answer = aiTextResponse;

            string voiceId = "EXAVITQu4vr4xnSDxMaL";
            string url = $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}/stream";

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("xi-api-key", _elevenLabsKey); 

            var payload = new
            {
                text = aiTextResponse,
                model_id = "eleven_multilingual_v2"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Answer = "Ses oluşturulamadı.";
                ViewBag.AudioUrl = null;
                return View();
            }

            return await SaveAndReturnAudio(response, aiTextResponse);
        }
        #endregion

        private async Task<IActionResult> SaveAndReturnAudio(HttpResponseMessage response, string? answerText = null)
        {
            var audioBytes = await response.Content.ReadAsByteArrayAsync();
            var fileName = $"voice_{Guid.NewGuid()}.mp3";
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "voices");
            var filePath = Path.Combine(directoryPath, fileName);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            await System.IO.File.WriteAllBytesAsync(filePath, audioBytes);

            ViewBag.AudioUrl = "/voices/" + fileName;
            if (answerText != null) ViewBag.Answer = answerText;

            return View();
        }
    }
}