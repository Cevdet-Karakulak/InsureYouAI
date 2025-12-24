using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace InsureYouAINew.Controllers
{
    public class TavilyController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _tavilyApiKey;
        private readonly string _openAIApiKey;

        // IConfiguration ile tüm anahtarları merkezi olarak yönetiyoruz
        public TavilyController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;

            // secrets.json hiyerarşisine uygun eşleştirmeler
            _tavilyApiKey = configuration["Tavily:ApiKey"];
            _openAIApiKey = configuration["OpenAI:ApiKey"];
        }

        [HttpGet]
        public IActionResult Search()
        {
            ViewBag.ControllerName = "Tavily AI";
            ViewBag.PageName = "Web Tarama & Özetleme";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchWithTavily(string query)
        {
            ViewBag.ControllerName = "Tavily AI";
            ViewBag.PageName = "Web Tarama & Özetleme";

            if (string.IsNullOrEmpty(query))
            {
                ViewBag.Error = "Lütfen bir arama sorgusu girin.";
                return View("Search");
            }

            try
            {
                // 1) Tavily web araması
                var tavilyResponse = await CallTavilyAsync(query);

                // 2) OpenAI ile sonuçların analiz edilmesi
                var openAIResponse = await SummarizeWithOpenAI(query, tavilyResponse);

                ViewBag.Query = query;
                ViewBag.TavilyRaw = tavilyResponse;
                ViewBag.OpenAIResult = openAIResponse;
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Bir hata oluştu: {ex.Message}";
            }

            return View("Search");
        }

        private async Task<string> CallTavilyAsync(string query)
        {
            var client = _httpClientFactory.CreateClient();
            var url = "https://api.tavily.com/search";

            var requestBody = new
            {
                api_key = _tavilyApiKey,
                query = query,
                include_answer = true,
                max_results = 5
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                return "Tavily arama sonuçları getirilemedi.";

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> SummarizeWithOpenAI(string query, string tavilyJson)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAIApiKey);

            var prompt = $@"
                    Kullanıcının sorusu: {query}

                    Aşağıdaki Tavily web araması sonuçlarını oku ve kullanıcıya kısa, net ve akademik bir açıklama yap.
                    Önemli noktaları sade şekilde özetle. Gereksiz cümle kurma.

                    Tavily sonuçları:
                    {tavilyJson}
                    ";

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = "Sen profesyonel bir sigorta ve teknoloji asistanısın." },
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"OpenAI Hatası: {response.StatusCode}";

            dynamic result = JsonConvert.DeserializeObject(responseString);
            return result?.choices?[0]?.message?.content ?? "Özet oluşturulamadı.";
        }
    }
}