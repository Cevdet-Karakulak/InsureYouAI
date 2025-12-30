using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace InsureYouAI.Controllers;

public class ArticleController : Controller
{
    private readonly InsureContext _context;
    public IConfiguration _configuration { get; set; }
    public ArticleController(InsureContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public IActionResult ArticleList()
    {
        ViewBag.ControllerName = "Makaleler";
        ViewBag.PageName = "Makale Listesi";
        var values = _context.Articles.Include(x => x.AppUser).Include(y => y.Category).ToList();
        return View(values);

    }
    [HttpGet]
    public IActionResult CreateArticle()
    {
        ViewBag.ControllerName = "Makaleler";
        ViewBag.PageName = "Yeni Makale Oluştur";

        ViewBag.Categories = _context.Categories
            .Select(x => new
            {
                x.CategoryId,
                x.CategoryName
            })
            .ToList();

        ViewBag.Authors = _context.Users
            .Select(x => new
            {
                x.Id,
                FullName = x.Name + " " + x.Surname
            })
            .ToList();
        return View();
    }
    [HttpPost]
    public IActionResult CreateArticle(Article Article)
    {
        Article.CreatedDate = DateTime.Now;
        _context.Articles.Add(Article);
        _context.SaveChanges();
        return RedirectToAction("ArticleList");
    }
    [HttpGet]
    public IActionResult UpdateArticle(int id)
    {
        ViewBag.ControllerName = "Makaleler";
        ViewBag.PageName = "Makale Güncelle";
        var article = _context.Articles
            .Include(x => x.AppUser)
            .FirstOrDefault(x => x.ArticleId == id);

        if (article == null) return NotFound();

        ViewBag.Categories = _context.Categories
            .Select(x => new { x.CategoryId, x.CategoryName })
            .ToList();

        ViewBag.Authors = _context.Users
            .Select(x => new { x.Id, FullName = x.Name + " " + x.Surname })
            .ToList();

        return View(article);
    }
    [HttpPost]
    public IActionResult UpdateArticle(Article model)
    {
        var article = _context.Articles.FirstOrDefault(x => x.ArticleId == model.ArticleId);
        if (article == null) return NotFound();

        article.Title = model.Title;
        article.Content = model.Content;
        article.CategoryId = model.CategoryId;
        article.CoverImageUrl = model.CoverImageUrl;
        article.MainCoverImageUrl = model.MainCoverImageUrl;

        _context.SaveChanges();
        return RedirectToAction("ArticleList");
    }



    public IActionResult DeleteArticle(int Id)
    {
        var value = _context.Articles.Find(Id);
        _context.Articles.Remove(value);
        _context.SaveChanges();
        return RedirectToAction("ArticleList");
    }


    [HttpGet]
    public IActionResult CreateArticleWithOpenAI()
    {
        ViewBag.ControllerName = "Makaleler";
        ViewBag.PageName = "Yapay Zeka Makale Oluşturucu";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateArticleWithOpenAI(string prompt)
    {
        var openAiKey = _configuration["OpenAI:ApiKey"];
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAiKey);

        var requestData = new
        {
            model = "gpt-4.1-mini",
            messages = new[]
            {
            new {role="system", content="Sen bir sigorta şirketi için çalışan, içerik yazarlığı yapan bir yapay zekasın. Kullanıcının verdiği özet ve anahtar kelimelere göre, sigortacılık sektörüyle ilgili makale üret. En az 5000 karakter olsun."},
            new {role="user", content=prompt}
        },
            temperature = 0.7
        };

        int retries = 5;          // Maksimum deneme sayısı
        int delayMs = 3000;       // 3 saniye bekleme süresi

        for (int i = 0; i < retries; i++)
        {
            var response = await client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
                var content = result.choices[0].message.content;
                ViewBag.article = content;
                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                // 429 geldi → bekle ve tekrar dene
                await Task.Delay(delayMs);
            }
            else
            {
                ViewBag.article = "Bir hata oluştu: " + response.StatusCode;
                return View();
            }
        }

        // Tüm denemeler başarısız olduysa
        ViewBag.article = "Bir hata oluştu: Çok fazla istek gönderildi. Lütfen biraz bekleyip tekrar deneyin.";
        return View();
    }


    public class OpenAIResponse
    {
        public List<Choice> choices { get; set; }
    }
    public class Choice
    {
        public Message message { get; set; }
    }
    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}
