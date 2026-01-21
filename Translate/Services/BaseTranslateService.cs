using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Translate.Helpers;

namespace Translate.Services
{
    /// <summary>
    /// Base class for translation services providing common validation, error handling, and helper methods.
    /// </summary>
    public abstract class BaseTranslateService : ITranslateService
    {
        protected readonly ILogger _logger;
        public abstract string Name { get; }

        protected BaseTranslateService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<TranslationResult> TranslateAsync(TranslationRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return new TranslationResult(false, null, "Please enter text to translate.");
            }

            var validationError = ValidateSettings();
            if (!string.IsNullOrWhiteSpace(validationError))
            {
                _logger.LogWarning("Translation settings invalid for {ServiceName}: {ValidationError}", Name, validationError);
                return new TranslationResult(false, null, validationError);
            }

            try
            {
                _logger.LogInformation("Starting translation with {ServiceName}. Source: {Source}, Target: {Target}, Length: {Length}",
                    Name, request.SourceLanguageCode, request.TargetLanguageCode, request.Text.Length);

                var result = await ExecuteTranslationAsync(request, cancellationToken);

                if (result.Success)
                {
                    _logger.LogInformation("Translation successful with {ServiceName}.", Name);
                }
                else
                {
                    _logger.LogWarning("Translation failed with {ServiceName}: {ErrorMessage}", Name, result.ErrorMessage);
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during translation with {ServiceName}.", Name);
                return new TranslationResult(false, null, $"Network error: {ex.Message}");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error during translation with {ServiceName}.", Name);
                return new TranslationResult(false, null, $"Invalid response format: {ex.Message}");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Translation cancelled for {ServiceName}.", Name);
                return new TranslationResult(false, null, "Translation cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during translation with {ServiceName}.", Name);
                return new TranslationResult(false, null, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates the service settings (e.g., API keys). Returns null if valid, or an error message if invalid.
        /// </summary>
        protected virtual string? ValidateSettings()
        {
            return null;
        }

        /// <summary>
        /// Executes the specific translation logic for the service.
        /// </summary>
        protected abstract Task<TranslationResult> ExecuteTranslationAsync(TranslationRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Generates the translation prompt using the default template.
        /// Can be overridden by subclasses if a different prompt format is needed.
        /// </summary>
        protected virtual string GeneratePrompt(TranslationRequest request)
        {
            return LanguageHelper.PromptTemplate
                .Replace("{SOURCE_LANG}", LanguageHelper.CodeToLanguage(request.SourceLanguageCode))
                .Replace("{SOURCE_CODE}", request.SourceLanguageCode)
                .Replace("{TARGET_LANG}", LanguageHelper.CodeToLanguage(request.TargetLanguageCode))
                .Replace("{TARGET_CODE}", request.TargetLanguageCode)
                .Replace("{TEXT}", request.Text);
        }
    }
}
