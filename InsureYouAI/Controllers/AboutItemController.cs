using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers;
public class AboutItemController : Controller
{
    private readonly InsureContext _context;
    public IConfiguration _configuration { get; set; }
    public AboutItemController(InsureContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public IActionResult AboutItemList()
    {
        ViewBag.ControllerName = "Hakkımızda";
        ViewBag.PageName = "Hakkımızda Ögeleri";
        var values = _context.AboutItems.ToList();
        return View(values);
    }
    [HttpGet]
    public IActionResult CreateAboutItem()
    {
        ViewBag.ControllerName = "Hakkımızda";
        ViewBag.PageName = "Yeni Hakkımızda Öge Girişi";
        return View();
    }
    [HttpPost]
    public IActionResult CreateAboutItem(AboutItem aboutItem)
    {
        _context.AboutItems.Add(aboutItem);
        _context.SaveChanges();
        return RedirectToAction("AboutItemList");
    }
    [HttpGet]
    public IActionResult UpdateAboutItem(int Id)
    {
        ViewBag.ControllerName = "Hakkımızda";
        ViewBag.PageName = "Hakkımızda Ögeleri Güncelleme";
        var value = _context.AboutItems.Find(Id);
        return View(value);
    }
    [HttpPost]
    public IActionResult UpdateAboutItem(AboutItem aboutItem)
    {
        _context.AboutItems.Update(aboutItem);
        _context.SaveChanges();
        return RedirectToAction("AboutItemList");
    }


    public IActionResult DeleteAboutItem(int Id)
    {
        var value = _context.AboutItems.Find(Id);
        _context.AboutItems.Remove(value);
        _context.SaveChanges();
        return RedirectToAction("AboutItemList");
    }

    [HttpGet]
    public async Task<IActionResult> CreateAboutItemWithGoogleGemini()
    {
        ViewBag.ControllerName = "Google Gemini";
        ViewBag.PageName = "Hakkımızda Öğeleri";
        
        var apiKey = _configuration["GeminiAI:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            ViewBag.Status = "NoKey";
            return View();
        }

        var model = "gemini-2.5-flash";
        var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
            new
            {
                parts = new[]
                {
                    new
                    {
                        text = "Kurumsal bir sigorta firması için güven verici, profesyonel en az 10 adet 'Hakkımızda öğesi' üret. Örnek: Güvenilirlik: Müşterilerimize karşı şeffaf davranırız."
                    }
                }
            }
        }
        };

        using var client = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        // 🔁 RETRY MEKANİZMASI (503 için)
        HttpResponseMessage response = null;
        for (int i = 0; i < 3; i++)
        {
            response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode) break;

            if ((int)response.StatusCode == 503)
                await Task.Delay(1200);
        }

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Status = response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable
                ? "Overloaded"
                : "Error";

            ViewBag.RawError = await response.Content.ReadAsStringAsync();
            return View();
        }

        var responseText = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(responseText);

        ViewBag.Status = "Success";
        ViewBag.Value = jsonDoc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return View();
    }

}
