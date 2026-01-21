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
    /// Translation service using OpenAI API.
    /// </summary>
    public class OpenAITranslateService : ITranslateService
    {
        private readonly HttpClient _http;
        private readonly SettingsService _settings;
        public string Name => "OpenAI";

        public OpenAITranslateService(HttpClient httpClient, SettingsService settings)
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

            if (string.IsNullOrWhiteSpace(_settings.OpenAIApiKey))
            {
                return new TranslationResult(false, null, "OpenAI API key is not configured.");
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
                
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = content;
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.OpenAIApiKey);

                using var response = await _http.SendAsync(requestMessage, cancellationToken);

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
