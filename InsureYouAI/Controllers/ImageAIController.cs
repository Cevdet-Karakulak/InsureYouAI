using InsureYouAI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class ImageAIController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ImageAIController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult CreateImageWithOpenAI()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateImageAjax([FromBody] ImagePromptDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Prompt))
                return BadRequest("Prompt boş.");

            var apiKey = _configuration["OpenAI:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return BadRequest("OpenAI API Key bulunamadı.");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-image-1",
                prompt = dto.Prompt,
                size = "1024x1024"
            };

            var response = await client.PostAsync(
                "https://api.openai.com/v1/images/generations",
                new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            var responseText = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                return BadRequest(responseText);

            using var doc = JsonDocument.Parse(responseText);
            var data = doc.RootElement.GetProperty("data")[0];

            byte[] imageBytes;

            // ✅ URL veya BASE64 destek
            if (data.TryGetProperty("url", out var urlProp))
            {
                var imageUrl = urlProp.GetString();
                imageBytes = await client.GetByteArrayAsync(imageUrl);
            }
            else if (data.TryGetProperty("b64_json", out var b64Prop))
            {
                imageBytes = Convert.FromBase64String(b64Prop.GetString());
            }
            else
            {
                return BadRequest("OpenAI görsel verisi bulunamadı.");
            }

            // ✅ wwwroot/AIImage kaydet
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AIImage");
            Directory.CreateDirectory(folder);

            var fileName = $"ai_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var filePath = Path.Combine(folder, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

            return Json(new { imageUrl = $"/AIImage/{fileName}" });
        }
        [HttpGet]
        public IActionResult Gallery()
        {
            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "AIImage"
            );

            var images = new List<string>();

            if (Directory.Exists(folderPath))
            {
                images = Directory.GetFiles(folderPath)
                    .OrderByDescending(f => f)
                    .Select(file => "/AIImage/" + Path.GetFileName(file))
                    .ToList();
            }

            return View(images);
        }
    }
}
