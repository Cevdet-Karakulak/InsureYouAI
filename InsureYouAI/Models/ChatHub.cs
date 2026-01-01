using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InsureYouAI.Models 
{
    public class ChatHub : Hub
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        
        private const string modelGpt = "gpt-4o-mini";

        private static readonly Dictionary<string, List<Dictionary<string, string>>> _history = new();

        public ChatHub(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public override Task OnConnectedAsync()
        {
            _history[Context.ConnectionId] =
                [
                new ()
                {
                    ["role"]="system",
                    ["content"]="You are a helpful assistant. Keep answers concise."
                }
                ];

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _history.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string userMessage)
        {
            await Clients.Caller.SendAsync("ReceiveUserEcho", userMessage);

            if (_history.TryGetValue(Context.ConnectionId, out var history))
            {
                if (history.Count > 20)
                    history.RemoveAt(1);
                
                history.Add(new() { ["role"] = "user", ["content"] = userMessage });
                
                await StreamOpenAI(history, Context.ConnectionAborted);
            }
        }

        private async Task StreamOpenAI(List<Dictionary<string, string>> history, CancellationToken cancellationToken)
        {
            string apiKey = _configuration["OpenAI:ApiKey"];
            
            if (string.IsNullOrEmpty(apiKey))
            {
                await Clients.Caller.SendAsync("ReceiveToken", "Hata: API anahtarı sunucuda bulunamadı.", cancellationToken);
                return;
            }

            
            var client = _httpClientFactory.CreateClient("openai"); 
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var payload = new
            {
                model = modelGpt,
                messages = history,
                stream = true,
                temperature = 0.7 
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, "v1/chat/completions");
            req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            
            if (!resp.IsSuccessStatusCode)
            {
                var errorErr = await resp.Content.ReadAsStringAsync(cancellationToken);
                await Clients.Caller.SendAsync("ReceiveToken", $"API Hatası: {errorErr}", cancellationToken);
                return;
            }

            using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            var sb = new StringBuilder();
            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!line.StartsWith("data:")) continue;

                var data = line["data:".Length..].Trim();
                if (data == "[DONE]") break;

                try
                {
                    var chunk = JsonSerializer.Deserialize<ChatStreamChunk>(data);
                    var delta = chunk?.Choices?.FirstOrDefault()?.Delta?.Content;
                    
                    if (!string.IsNullOrEmpty(delta))
                    {
                        
                        sb.Append(delta); 
                        await Clients.Caller.SendAsync("ReceiveToken", delta, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Streaming error: {ex.Message}");
                }
            }

            var full = sb.ToString();
            
            history.Add(new() { ["role"] = "assistant", ["content"] = full });
            
            await Clients.Caller.SendAsync("CompleteMessage", full, cancellationToken);
        }

        private sealed class ChatStreamChunk
        {
            [JsonPropertyName("choices")] public List<Choice>? Choices { get; set; }
        }

        private sealed class Choice
        {
            [JsonPropertyName("delta")] public Delta? Delta { get; set; }
            [JsonPropertyName("finish_reason")] public string? FinishReason { get; set; }
        }

        private sealed class Delta
        {
            [JsonPropertyName("content")] public string? Content { get; set; }
            [JsonPropertyName("role")] public string? Role { get; set; }
        }
    }
}
