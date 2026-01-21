using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Translate.Services
{
    /// <summary>
    /// Translation service using OpenAI API.
    /// </summary>
    public class OpenAITranslateService : BaseTranslateService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingsService _settings;

        public override string Name => "OpenAI";

        public OpenAITranslateService(IHttpClientFactory httpClientFactory, SettingsService settings, Microsoft.Extensions.Logging.ILogger<OpenAITranslateService> logger)
            : base(logger)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings;
        }

        protected override string? ValidateSettings()
        {
            return string.IsNullOrWhiteSpace(_settings.OpenAIApiKey) ? "OpenAI API key is not configured." : null;
        }

        protected override async Task<TranslationResult> ExecuteTranslationAsync(TranslationRequest request, CancellationToken cancellationToken)
        {
            string prompt = GeneratePrompt(request);

            var body = new
            {
                model = _settings.OpenAIModel,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                stream = false
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"{_settings.OpenAIBaseUrl.TrimEnd('/')}/chat/completions";

            using var httpClient = _httpClientFactory.CreateClient("OpenAI");
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Content = content;
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.OpenAIApiKey);

            using var response = await httpClient.SendAsync(requestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return new TranslationResult(false, null, $"Translation failed: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(result);

            // Parse OpenAI response structure
            var choices = doc.RootElement.GetProperty("choices");
            if (choices.GetArrayLength() == 0)
            {
                return new TranslationResult(false, null, "No translation candidates returned.");
            }

            var translation = choices[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(translation))
            {
                return new TranslationResult(false, null, "No translation returned.");
            }

            return new TranslationResult(true, translation.Trim(), null);
        }
    }
}
