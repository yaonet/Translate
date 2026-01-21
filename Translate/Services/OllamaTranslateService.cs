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
    public class OllamaTranslateService : BaseTranslateService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingsService _settings;

        public override string Name => "Ollama";

        public OllamaTranslateService(IHttpClientFactory httpClientFactory, SettingsService settings, Microsoft.Extensions.Logging.ILogger<OllamaTranslateService> logger)
            : base(logger)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings;
        }

        protected override async Task<TranslationResult> ExecuteTranslationAsync(TranslationRequest request, CancellationToken cancellationToken)
        {
            string prompt = GeneratePrompt(request);

            var body = new
            {
                model = _settings.OllamaModel,
                prompt = prompt!,
                stream = false
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var httpClient = _httpClientFactory.CreateClient("Ollama");
            using var response = await httpClient.PostAsync($"{_settings.OllamaBaseUrl}/api/generate", content, cancellationToken);
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
    }
}
