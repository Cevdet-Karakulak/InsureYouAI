using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers;
public class AboutController : Controller
{
    private readonly InsureContext _context;
    public IConfiguration _configuration { get; set; }
    public AboutController(InsureContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public IActionResult AboutList()
    {
        ViewBag.ControllerName = "Hakkımızda";
        ViewBag.PageName = "Mevcut Hakkımızda Yazısı";

        var values = _context.Abouts.ToList();
        ViewBag.HasAbout = values.Any();

        return View(values);
    }

    [HttpGet]
    public IActionResult CreateAbout(bool force = false)
    {
        ViewBag.ControllerName = "Hakkımızda";
        ViewBag.PageName = "Yeni Hakkımızda Yazı Girişi (Tema bütünlüğü için yalnızca 1 adet Hakkımızda yazısı giriniz)";

        if (force)
        {
            var existing = _context.Abouts.FirstOrDefault();
            if (existing != null)
            {
                _context.Abouts.Remove(existing);
                _context.SaveChanges();
            }
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateAbout(About about)
    {
        _context.Abouts.Add(about);
        _context.SaveChanges();

        TempData["Success"] = "Hakkımızda yazısı başarıyla kaydedildi.";
        return RedirectToAction("AboutList");
    }
    [HttpGet]
    public IActionResult UpdateAbout(int Id)
    {
        ViewBag.ControllerName = "Hakkımızda";
        ViewBag.PageName = "Hakkımızda Yazı Güncelleme Sayfası";
        var value = _context.Abouts.Find(Id);
        return View(value);
    }
    [HttpPost]
    public IActionResult UpdateAbout(About about)
    {
        _context.Abouts.Update(about);
        _context.SaveChanges();
        return RedirectToAction("AboutList");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteExistingAbout()
    {
        var existing = _context.Abouts.FirstOrDefault();
        if (existing != null)
        {
            _context.Abouts.Remove(existing);
            _context.SaveChanges();
        }

        return RedirectToAction("CreateAbout");
    }
    [HttpGet]
    public IActionResult DeleteAbout(int Id)
    {
        var value = _context.Abouts.Find(Id);
        _context.Abouts.Remove(value);
        _context.SaveChanges();
        return RedirectToAction("AboutList");
    }

    [HttpGet]
    public async Task<IActionResult> CreateAboutWithGoogleGemini()
    {
        var geminiKey = _configuration["GeminiAI:ApiKey"];
        var model = "gemini-1.5-pro";
        var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={geminiKey}";
        var requestBody = new
        {
            contents = new[]
            {
                    new
                    {
                        parts=new[]
                        {
                            new
                            {
                                text="Kurumsal bir sigorta firması için etkileyici, güven verici ve profesyonel bir 'Hakkımızda' yazısı oluştur."
                            }
                        }
                    }
                }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(url, content);
        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(responseJson);
            var aboutText = jsonDoc.RootElement
                                 .GetProperty("candidates")[0]
                                 .GetProperty("content")
                                 .GetProperty("parts")[0]
                                 .GetProperty("text")
                                 .GetString();

            ViewBag.value = aboutText;
       
        }
        else
        {
            ViewBag.value = "Gemini API yanıtı başarısız";
        }

            return View();
    }
}
