using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class ServiceController : Controller
    {
        private readonly InsureContext _context;
        private readonly IConfiguration _configuration; 
        public ServiceController(InsureContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult ServiceList()
        {
            var values = _context.Services.ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateService()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateService(Service service)
        {
            _context.Services.Add(service);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");
        }

        [HttpGet]
        public IActionResult UpdateService(int id)
        {
            var value = _context.Services.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateService(Service service)
        {
            _context.Services.Update(service);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");
        }

        public IActionResult DeleteService(int id)
        {
            var value = _context.Services.Find(id);
            if (value != null)
            {
                _context.Services.Remove(value);
                _context.SaveChanges();
            }
            return RedirectToAction("ServiceList");
        }

        public async Task<IActionResult> CreateServiceWithAnthropicClaude()
        {
            string apiKey = _configuration["Anthropic:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                ViewBag.services = new List<string> { "API Anahtarı bulunamadı. Lütfen yapılandırmayı kontrol edin." };
                return View();
            }

            string prompt = "Bir sigorta şirketi için hizmetler bölümü hazırlamanı istiyorum. Burada 5 farklı hizmet olmalı. Bana maksimum 100 karakterden oluşan cümlelerle 5 tane hizmet içeriği yazar mısın?";

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.anthropic.com/");
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestBody = new
            {
                model = "claude-3-opus-20240229",
                max_tokens = 512,
                temperature = 0.5,
                messages = new[]
                {
                    new
                    {
                        role="user",
                        content=prompt
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json"); // Encoding eklendi
            var response = await client.PostAsync("v1/messages", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.services = new List<string>
                {
                    $"Claude Api'den Cevap Alınamadı. Durum Kodu: {response.StatusCode}. Detay: {errorContent}"
                };
                return View();
            }

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);

            var fullText = doc.RootElement
                            .GetProperty("content")[0]
                            .GetProperty("text")
                            .GetString();

            var services = fullText.Split('\n')
                                     .Where(x => !string.IsNullOrEmpty(x))
                                     .Select(x => x.TrimStart('1', '2', '3', '4', '5', '.', ' ', '-')) 
                                     .ToList();
            ViewBag.services = services;

            return View();
        }
    }
}
