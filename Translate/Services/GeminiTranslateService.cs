using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Translate.Helpers;

namespace Translate.Services
{
    /// <summary>
    /// Translation service using Google Gemini API.
    /// </summary>
    public class GeminiTranslateService : ITranslateService
    {
        private readonly HttpClient _http;
        private readonly SettingsService _settings;
        public string Name => "Gemini";

        public GeminiTranslateService(HttpClient httpClient, SettingsService settings)
        {
            _http = httpClient;
            _settings = settings;
        }
        public async Task<TranslationResult> TranslateAsync(TranslationRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return new TranslationResult(false, null, "Please enter text to translate.");
            }

            if (string.IsNullOrWhiteSpace(_settings.GeminiApiKey))
            {
                return new TranslationResult(false, null, "Gemini API key is not configured.");
            }

            try
            {
                string prompt = LanguageHelper.PromptTemplate
                    .Replace("{SOURCE_LANG}", LanguageHelper.CodeToLanguage(request.SourceLanguageCode))
                    .Replace("{SOURCE_CODE}", request.SourceLanguageCode)
                    .Replace("{TARGET_LANG}", LanguageHelper.CodeToLanguage(request.TargetLanguageCode))
                    .Replace("{TARGET_CODE}", request.TargetLanguageCode)
                    .Replace("{TEXT}", request.Text);

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

                var url = $"{_settings.GeminiBaseUrl.TrimEnd('/')}/v1beta/models/{_settings.GeminiModel}:generateContent?key={_settings.GeminiApiKey}";
                using var response = await _http.PostAsync(url, content, cancellationToken);

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
            catch (HttpRequestException ex)
            {
                return new TranslationResult(false, null, $"Network error: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return new TranslationResult(false, null, $"Invalid response format: {ex.Message}");
            }
            catch (OperationCanceledException)
            {
                return new TranslationResult(false, null, "Translation cancelled.");
            }
            catch (Exception ex)
            {
                return new TranslationResult(false, null, $"Error: {ex.Message}");
            }
        }
    }
}
