using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Translate.Services
{
    /// <summary>
    /// Translation service using Google Gemini API.
    /// </summary>
    public class GeminiTranslateService(IHttpClientFactory httpClientFactory, SettingsService settings, Microsoft.Extensions.Logging.ILogger<GeminiTranslateService> logger) : BaseTranslateService(logger)
    {
        public override string Name => "Gemini";

        protected override string? ValidateSettings()
        {
            return string.IsNullOrWhiteSpace(settings.GeminiApiKey) ? "Gemini API key is not configured." : null;
        }

        protected override async Task<TranslationResult> ExecuteTranslationAsync(TranslationRequest request, CancellationToken cancellationToken)
        {
            string prompt = GeneratePrompt(request);

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"{settings.GeminiBaseUrl.TrimEnd('/')}/v1beta/models/{settings.GeminiModel}:generateContent";

            using var httpClient = httpClientFactory.CreateClient("Gemini");
            
            // Use header instead of URL parameter for better security
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.Add("x-goog-api-key", settings.GeminiApiKey);
            requestMessage.Content = content;

            using var response = await httpClient.SendAsync(requestMessage, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return new TranslationResult(false, null, $"Translation failed: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(result);

            // Parse Gemini response structure
            var candidates = doc.RootElement.GetProperty("candidates");
            if (candidates.GetArrayLength() == 0)
            {
                return new TranslationResult(false, null, "No translation candidates returned.");
            }

            var translation = candidates[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(translation))
            {
                return new TranslationResult(false, null, "No translation returned.");
            }

            return new TranslationResult(true, translation.Trim(), null);
        }
    }
}
