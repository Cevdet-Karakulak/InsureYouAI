using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace InsureYouAI.Services
{
    public class AiMessageClassifierService
    {
        private readonly IConfiguration _configuration;

        public AiMessageClassifierService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> DetectCategoryAsync(string messageText)
        {
            try
            {
                var apiKey = _configuration["GeminiAI:ApiKey"];
                var model = "gemini-2.5-flash";
                var url =
                    $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";

                var prompt = $@"
Aşağıdaki mesaj hangi sigorta türü ile ilgilidir?
SADECE listeden BİR TANESİNİ yaz.

Sağlık Sigortası
Yurt Dışı Seyahat Sigortası
Araç Sigortası
Konut Sigortası
Kasko Sigortası
Trafik Sigortası
İş Yeri Sigortası
Hayat Sigortası
Tamamlayıcı Sağlık Sigortası
Evcil Hayvan Sigortası
Eğitim Sigortası
DASK (Zorunlu Deprem)
Ferdi Kaza Sigortası
Siber Güvenlik Sigortası
Nakliyat Sigortası
Tarım Sigortası

MESAJ:
{messageText}
";

                var body = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = prompt } } }
                    }
                };

                using var client = new HttpClient();
                var response = await client.PostAsync(
                    url,
                    new StringContent(
                        JsonSerializer.Serialize(body),
                        Encoding.UTF8,
                        "application/json"));

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("candidates", out var candidates))
                    return FallbackCategory(messageText);

                var text = candidates[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString()
                    ?.ToLower();

                if (string.IsNullOrEmpty(text))
                    return FallbackCategory(messageText);

                if (text.Contains("sağlık"))
                    return "Sağlık Sigortası";
                if (text.Contains("kasko"))
                    return "Kasko Sigortası";
                if (text.Contains("trafik"))
                    return "Trafik Sigortası";
                if (text.Contains("dask"))
                    return "DASK (Zorunlu Deprem)";
                if (text.Contains("konut"))
                    return "Konut Sigortası";
                if (text.Contains("hayat"))
                    return "Hayat Sigortası";
                if (text.Contains("seyahat"))
                    return "Yurt Dışı Seyahat Sigortası";

                return FallbackCategory(messageText);
            }
            catch
            {
                return FallbackCategory(messageText);
            }
        }

        private string FallbackCategory(string message)
        {
            message = message.ToLower();

            if (
                message.Contains("hastane") ||
                message.Contains("muayene") ||
                message.Contains("ameliyat") ||
                message.Contains("tahlil")
            )
                return "Sağlık Sigortası";

            if (message.Contains("kasko"))
                return "Kasko Sigortası";

            if (message.Contains("trafik"))
                return "Trafik Sigortası";

            if (message.Contains("dask") || message.Contains("deprem"))
                return "DASK (Zorunlu Deprem)";

            if (message.Contains("ev") || message.Contains("konut"))
                return "Konut Sigortası";

            return "Bilinmiyor";
        }

        public Task<string> DetectPriorityAsync(string message)
        {
            message = message.ToLower();

            if (
                message.Contains("acil") ||
                message.Contains("kaza") ||
                message.Contains("hasar") ||
                message.Contains("ameliyat")
            )
                return Task.FromResult("Acil");

            if (message.Contains("bilgi") || message.Contains("öğrenmek"))
                return Task.FromResult("Orta");

            return Task.FromResult("Düşük");
        }
    }
}
