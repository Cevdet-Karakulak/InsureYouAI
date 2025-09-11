using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace InsureYouAI.Controllers;

public class TestimonialController : Controller
{
    private readonly InsureContext _context;
    private readonly IConfiguration _configuration;
    public TestimonialController(InsureContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public IActionResult TestimonialList()
    {
        var values = _context.Testimonials.ToList();
        return View(values);
    }
    [HttpGet]
    public IActionResult CreateTestimonial()
    {
        return View();
    }
    [HttpPost]
    public IActionResult CreateTestimonial(Testimonial testimonial)
    {
        _context.Testimonials.Add(testimonial);
        _context.SaveChanges();
        return RedirectToAction("TestimonialList");
    }
    [HttpGet]
    public IActionResult UpdateTestimonial(int Id)
    {
        var value = _context.Testimonials.Find(Id);
        return View(value);
    }
    [HttpPost]
    public IActionResult UpdateTestimonial(Testimonial testimonial)
    {
        _context.Testimonials.Update(testimonial);
        _context.SaveChanges();
        return RedirectToAction("TestimonialList");
    }


    public IActionResult DeleteTestimonial(int Id)
    {
        var value = _context.Testimonials.Find(Id);
        _context.Testimonials.Remove(value);
        _context.SaveChanges();
        return RedirectToAction("TestimonialList");
    }

    public async Task<IActionResult> CreateTestimonialWithClaudeAI()
    {
        string apiKey = "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";


        string prompt = "Bir sigorta şirketi için müşteri deneyimlerine dair yorum oluşturmak istiyorum yani İngilizce karşılığı ile: testimonial. Bu alanda Türkçe olarak 6 tane yorum, 6 tane müşteri adı ve soyadı, bu müşterilerin unvanı olsun. Buna göre içeriği hazırla.";

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

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody));
        var response = await client.PostAsync("v1/messages", jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.testimonials = new List<string>
                {
                    $"Claude Api'den Cevap Alınamadı. Hata: {response.StatusCode}"
                };
            return View();
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);

        var fullText = doc.RootElement
                        .GetProperty("content")[0]
                        .GetProperty("text")
                        .GetString();

        var testimonials = fullText.Split('\n')
                             .Where(x => !string.IsNullOrEmpty(x))
                             .Select(x => x.TrimStart('1', '2', '3', '4', '5', '.', ' '))
                             .ToList();
        ViewBag.testimonials = testimonials;

        return View();
    }
}
