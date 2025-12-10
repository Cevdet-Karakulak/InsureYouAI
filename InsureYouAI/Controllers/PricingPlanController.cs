using InsureYouAI.Context;
using InsureYouAI.Entities;
using InsureYouAI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace InsureYouAINew.Controllers
{
    public class PricingPlanController : Controller
    {
        private readonly InsureContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public PricingPlanController(
            InsureContext context,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult PricingPlanList()
        {
            ViewBag.ControllerName = "AI Destekli Sigorta Planı";
            ViewBag.PageName = "Mevcut Sigorta Plan Listeleri";
            var values = _context.PricingPlans.ToList();
            return View(values);

        }
        [HttpGet]
        public IActionResult CreatePricingPlan()
        {
            ViewBag.ControllerName = "AI Destekli Sigorta Planı";
            ViewBag.PageName = "Yeni Sigorta Planı Oluşturma";
            return View();
        }
        [HttpPost]
        public IActionResult CreatePricingPlan(PricingPlan pricingPlan)
        {
            _context.PricingPlans.Add(pricingPlan);
            _context.SaveChanges();
            return RedirectToAction("PricingPlanList");
        }


        [HttpGet]
        public IActionResult CreateUserCustomizePlan()
        {
            ViewBag.ControllerName = "AI Destekli Sigorta Planı";
            ViewBag.PageName = "Kullanıcıya Özel AI Destekli Sigorta Planı";
            return View(new AIInsuranceRecommendationViewModel());
        }
        [HttpGet]
        public IActionResult UpdatePricingPlan(int id)
        {
            ViewBag.ControllerName = "AI Destekli Sigorta Planı";
            ViewBag.PageName = "Sigorta Plan Revizyonu";
            var value = _context.PricingPlans.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdatePricingPlan(PricingPlan pricingPlan)
        {
            _context.PricingPlans.Update(pricingPlan);
            _context.SaveChanges();
            return RedirectToAction("PricingPlanList");
        }

        public IActionResult DeletePricingPlan(int id)
        {
            var value = _context.PricingPlans.Find(id);
            _context.PricingPlans.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("PricingPlanList");
        }

        public IActionResult ChangeStatus(int id)
        {
            var value = _context.PricingPlans.Find(id);
            if (value.IsFeature == true)
            {
                value.IsFeature = false;
            }
            else
            {
                value.IsFeature = true;
            }
            _context.SaveChanges();
            return RedirectToAction("PricingPlanList");
        }


        [HttpPost]
        public async Task<IActionResult> CreateUserCustomizePlan(AIInsuranceRecommendationViewModel model)
        {
            ViewBag.ControllerName = "AI Destekli Sigorta Planı";
            ViewBag.PageName = "Kullanıcıya Özel AI Destekli Sigorta Planı";

            string apiKey = _configuration["OpenAI:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                ModelState.AddModelError("", "OpenAI API Key bulunamadı.");
                return View(model);
            }

            var userJson = JsonConvert.SerializeObject(model);

            string prompt = $@"
Sen profesyonel bir sigorta uzmanı AI asistanısın.

Paketler:
- Premium (599 TL): geniş kapsam, yurtiçi/yurtdışı
- Standart (449 TL): orta seviye teminat
- Ekonomik (339 TL): temel teminat

Kullanıcı Profili:
{userJson}

Sadece GEÇERLİ JSON döndür:
{{
 ""onerilenPaket"": ""Premium | Standart | Ekonomik"",
 ""ikinciSecenek"": ""Premium | Standart | Ekonomik"",
 ""neden"": ""Kısa analiz""
}}
";

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-4.1-mini",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var response = await client.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "AI servisinden yanıt alınamadı.");
                return View(model);
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            dynamic ai = JsonConvert.DeserializeObject(jsonResponse);
            string aiText = ai?.choices?[0]?.message?.content;

            if (string.IsNullOrWhiteSpace(aiText))
            {
                ModelState.AddModelError("", "AI cevabı boş döndü.");
                return View(model);
            }

            var result = JsonConvert.DeserializeObject<AIInsuranceRecommendationViewModel>(aiText);

            model.RecommendedPackage = result?.onerilenPaket;
            model.SecondBestPackage = result?.ikinciSecenek;
            model.AnalysisText = result?.neden;

            TempData["RawAI"] = aiText;
            return View(model);
        }
    }
}
