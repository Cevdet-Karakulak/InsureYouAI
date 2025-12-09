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
        ViewBag.PageName = "Hakkımızda Maddeler";
        var values = _context.AboutItems.ToList();
        return View(values);
    }
    [HttpGet]
    public IActionResult CreateAboutItem()
    {
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
        var apiKey = "AIzaSyCU20aFvDdvLqL4EdaULcFXbTcSExVwOwo";
        var model = "gemini-1.5-pro";
        var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";
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
                                text="Kurumsal bir sigorta firması için etkileyici, güven verici ve profesyonel bir 'Hakkımızda alanları(About Item)' yazısı oluştur. Örneğin 'Güvenilirlik: Müşterilerimize karşı her zaman dürüst ve şeffaf bir iletişim kurarız.' şeklinde veya bunun gibi ve buna benzer daha zengin içerikler gelsin. en az 10 tane item istiyorum."
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
