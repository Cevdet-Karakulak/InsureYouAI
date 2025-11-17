using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class BlogController : Controller
    {
        private readonly InsureContext _context;
        private readonly IConfiguration _config;

        public BlogController(InsureContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public IActionResult BlogList()
        {
            return View();
        }

        public IActionResult GetBlogByCategory(int id)
        {
            ViewBag.c = id;
            return View();
        }

        public IActionResult BlogDetail(int id)
        {
            var article = _context.Articles
                .Include(a => a.AppUser)
                .Include(a => a.Category)
                .Include(a => a.Comments)
                .FirstOrDefault(a => a.ArticleId == id);

            ViewBag.i = id; 
            return View(article);
        }

        public PartialViewResult GetBlog()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult GetBlog(string keyword)
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult AddComment()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            comment.AppUserId = "f454e769-3a98-4358-ad3e-42ccc79973a7";

            var apiKey = _config["HuggingFace:ApiKey"];
            var toxicUrl = _config["HuggingFace:ModelUrl"];
            var translateUrl = _config["HuggingFace:TranslateUrl"];

            var toxicWords = new List<string>
            {
                "orospu", "amk", "amına", "siktir", "piç", "yarrak",
                "kahpe", "gavat", "pezevenk", "it oğlu it", "sikerim",
                "bokka", "bok herif", "şerefsiz",

                "gerizekalı", "aptal", "salak", "embesil", "dangalak",
                "malsın", "düşüncesiz", "geri zekalı", "hödük",

                "iğrenç", "iğrençlik", "rezil", "kepaze", "utanç verici",
                "dangalak", "alçak", "aşağılık", "karaktersiz", "hain",
                "şarlatan", "soytarı", "yalancı", "ikiyüzlü", "sahtekar",

                "insanlığa ayıp", "insanlığa hakaret", "varlığın utanç",
                "yüz karası", "ayıpsın", "utanç kaynağı",

                "nefret ediyorum", "senden tiksiniyorum", "tiksinç",
                "lanet olsun", "yok ol", "defol git",

                "değersiz", "çöp", "çöp gibi", "hiçsin", "bok gibi",
                "küçüksün", "zavallı", "acınası", "yetersiz", "ezik",
                "basit insan", "masılsın", "kişiliksiz",

                "gebersin", "gebersen iyi olur", "öl artık",
                "seni yok etsinler", "lanet insan",

                "namussuz", "haysiyetsiz", "onursuz",
            };

            string lower = comment.CommentDetail.ToLower();

            if (toxicWords.Any(w => lower.Contains(w)))
            {
                comment.CommentStatus = "Toksik Yorum";
            }

            if (string.IsNullOrEmpty(comment.CommentStatus))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                    try { 
                        var translateBody = new { inputs = comment.CommentDetail };
                        var translateJson = JsonSerializer.Serialize(translateBody);
                        var translateContent = new StringContent(translateJson, Encoding.UTF8, "application/json");

                        var translateResponse = await client.PostAsync(translateUrl, translateContent);
                        var translateString = await translateResponse.Content.ReadAsStringAsync();

                        string englishText = comment.CommentDetail;

                        if (translateString.TrimStart().StartsWith("["))
                        {
                            var doc = JsonDocument.Parse(translateString);
                            englishText = doc.RootElement[0].GetProperty("translation_text").GetString();
                        }

                        var toxicBody = new { inputs = englishText };
                        var toxicJson = JsonSerializer.Serialize(toxicBody);
                        var toxicContent = new StringContent(toxicJson, Encoding.UTF8, "application/json");

                        var toxicResponse = await client.PostAsync(toxicUrl, toxicContent);
                        var toxicString = await toxicResponse.Content.ReadAsStringAsync();

                        if (toxicString.TrimStart().StartsWith("["))
                        {
                            var toxicDoc = JsonDocument.Parse(toxicString);
                            foreach (var item in toxicDoc.RootElement[0].EnumerateArray())
                            {
                                string label = item.GetProperty("label").GetString();
                                double score = item.GetProperty("score").GetDouble();

                                if (score > 0.5)
                                {
                                    comment.CommentStatus = "Toksik Yorum";
                                    break;
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(comment.CommentStatus))
                        {
                            comment.CommentStatus = "Yorum Onaylandı";
                        }
                    }
                    catch
                    {
                        comment.CommentStatus = "Onay Bekliyor";
                    }
                }
            }

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("BlogDetail", new { id = comment.ArticleId });
        }
    }
}
