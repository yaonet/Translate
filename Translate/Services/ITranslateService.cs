using System.Threading;
using System.Threading.Tasks;

namespace Translate.Services
{
    /// <summary>
    /// Translation request containing the text and language codes.
    /// </summary>
    public record TranslationRequest(string Text, string SourceLanguageCode, string TargetLanguageCode);

    /// <summary>
    /// Translation result containing the translated text or error information.
    /// </summary>
    public record TranslationResult(bool Success, string? TranslatedText, string? ErrorMessage);

    /// <summary>
    /// Interface for translation services. Implement this interface to add new translation providers.
    /// </summary>
    public interface ITranslateService
    {
        /// <summary>
        /// Display name of the translation service.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Translates text based on the request.
        /// </summary>
        /// <param name="request">Translation request containing text and language codes.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Translation result containing the translated text or error.</returns>
        Task<TranslationResult> TranslateAsync(TranslationRequest request, CancellationToken cancellationToken = default);
    }
}
