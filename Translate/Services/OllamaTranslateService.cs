using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Translate.Services
{
    /// <summary>
    /// Translation service using Ollama local LLM.
    /// </summary>
    public class OllamaTranslateService : ITranslateService
    {
        // https://ollama.com/library/translategemma
        private const string PromptTemplate = """
            You are a professional {SOURCE_LANG} ({SOURCE_CODE}) to {TARGET_LANG} ({TARGET_CODE}) translator. 
            Your goal is to accurately convey the meaning and nuances of the original {SOURCE_LANG} text 
            while adhering to {TARGET_LANG} grammar, vocabulary, and cultural sensitivities.
            Produce only the {TARGET_LANG} translation, without any additional explanations or commentary. 
            Please translate the following {SOURCE_LANG} text into {TARGET_LANG}:


            {TEXT}
            """;

        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly string _model;

        public string Name => "Ollama";

        public OllamaTranslateService(HttpClient httpClient, string baseUrl = "http://localhost:11434", string model = "translategemma")
        {
            _http = httpClient;
            _baseUrl = baseUrl;
            _model = model;
        }

        public async Task<TranslationResult> TranslateAsync(TranslationRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return new TranslationResult(false, null, "Please enter text to translate.");
            }

            try
            {
                string sourceLanguage = CodeToLanguage(request.SourceLanguageCode);
                string targetLanguage = CodeToLanguage(request.TargetLanguageCode);

                string prompt = PromptTemplate
                    .Replace("{SOURCE_LANG}", sourceLanguage)
                    .Replace("{SOURCE_CODE}", request.SourceLanguageCode)
                    .Replace("{TARGET_LANG}", targetLanguage)
                    .Replace("{TARGET_CODE}", request.TargetLanguageCode)
                    .Replace("{TEXT}", request.Text);

                var body = new
                {
                    model = _model,
                    prompt = prompt!,
                    stream = false
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await _http.PostAsync($"{_baseUrl}/api/generate", content, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    return new TranslationResult(false, null, $"Translation failed: {response.StatusCode}");
                }

                var result = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(result);

                var translation = doc.RootElement.GetProperty("response").GetString();
                if (string.IsNullOrWhiteSpace(translation))
                {
                    return new TranslationResult(false, null, "No translation returned.");
                }

                return new TranslationResult(true, translation, null);
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

        private static string CodeToLanguage(string code)
        {
            return code switch
            {
                "en" => "English",
                "zh" => "Chinese",
                "ja" => "Japanese",
                "ko" => "Korean",
                _ => code
            };
        }


    }
}
