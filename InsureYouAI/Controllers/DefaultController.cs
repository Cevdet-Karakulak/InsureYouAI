using InsureYouAI.Context;
using InsureYouAI.Entities;
using InsureYouAI.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace InsureYouAI.Controllers
{
    public class DefaultController : Controller
    {
        private readonly InsureContext _context;
        private readonly AiMessageClassifierService _aiService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _http;

        public DefaultController(
            InsureContext context,
            AiMessageClassifierService aiService,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _aiService = aiService;
            _configuration = configuration;
            _http = httpClientFactory.CreateClient();
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message)
        {
            // Temel bilgiler
            message.SendDate = DateTime.Now;
            message.IsRead = false;

            // AI sınıflandırma
            message.AiCategory = await _aiService.DetectCategoryAsync(message.MessagetDetail);
            message.Priority = await _aiService.DetectPriorityAsync(message.MessagetDetail);

            _context.Messages.Add(message);
            _context.SaveChanges();

            // ---------------------------------------------------
            //  G E M I N I   A I   -   M Ü Ş T E R İ   Y A N I T I
            // ---------------------------------------------------
            string apiKey = _configuration["GeminiAI:ApiKey"];
            string model = "gemini-2.5-flash-latest";

            string prompt =
$@"Sigorta firması müşteri temsilcisi gibi profesyonel bir yanıt hazırla.

Kurallar:
- 2–3 paragraf yaz.
- Eksik bilgi varsa kibarca iste.
- Fiyat / rakam verme.
- Empati kur.
- Sonunda mutlaka imza ekle:
Saygılarımızla,
InsureYou Sigorta
Müşteri İletişim Asistanı

Aşağıdaki formatta TEK SATIRLIK JSON döndür:
{{""subject"":""..."",""body"":""...""}}

Müşteri mesajı:
{message.MessagetDetail}

Kategori: {message.AiCategory}
Öncelik: {message.Priority}
";

            var body = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            string subject = "InsureYouAI Yanıtı";
            string textBody = "Talebiniz alınmıştır.";

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

                var response = await _http.PostAsync(
                    $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}",
                    content);

                string result = await response.Content.ReadAsStringAsync();

                var doc = JsonDocument.Parse(result);
                string aiText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                // JSON’ı parse et
                var clean = aiText.Replace("\n", " ").Trim();
                var parsed = JsonNode.Parse(clean);

                subject = parsed?["subject"]?.ToString() ?? subject;
                textBody = parsed?["body"]?.ToString() ?? textBody;
            }
            catch
            {
                // Hata olsa bile mail göndersin
                textBody = "Talebiniz alınmıştır. En kısa zamanda dönüş yapılacaktır.";
            }

            // ---------------------------------------------------
            //  M A İ L   G Ö N D E R
            // ---------------------------------------------------
            try
            {
                var mail = new MimeMessage();
                mail.From.Add(new MailboxAddress("InsureYouAI", _configuration["Email:From"]));
                mail.To.Add(new MailboxAddress(message.NameSurname, message.Email));
                mail.Subject = subject;
                mail.Body = new BodyBuilder { TextBody = textBody }.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, false);
                smtp.Authenticate(_configuration["Email:From"], _configuration["Email:Password"]);
                smtp.Send(mail);
                smtp.Disconnect(true);
            }
            catch
            {
                // email hatası sistemi bozmasın
            }

            // ---------------------------------------------------
            //  DB LOG KAYDI
            // ---------------------------------------------------
            _context.ClaudeAIMessages.Add(new ClaudeAIMessage
            {
                MessageDetail = textBody,
                ReceiveEmail = message.Email,
                ReceiveNameSurname = message.NameSurname,
                SendDate = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { sent = true });
        }
    }
}
