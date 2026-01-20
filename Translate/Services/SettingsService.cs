using Windows.Storage;

namespace Translate.Services
{
    /// <summary>
    /// Service for managing application settings using LocalSettings.
    /// </summary>
    public class SettingsService
    {
        private readonly ApplicationDataContainer _localSettings;

        // Setting keys
        private const string GeminiApiKeyKey = "GeminiApiKey";
        private const string GeminiModelKey = "GeminiModel";
        private const string OllamaBaseUrlKey = "OllamaBaseUrl";
        private const string OllamaModelKey = "OllamaModel";

        // Default values
        private const string DefaultGeminiModel = "gemini-flash-latest";
        private const string DefaultOllamaBaseUrl = "http://localhost:11434";
        private const string DefaultOllamaModel = "translategemma";

        public SettingsService()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
        }

        #region Gemini Settings

        public string GeminiApiKey
        {
            get => GetSetting(GeminiApiKeyKey, string.Empty);
            set => SetSetting(GeminiApiKeyKey, value);
        }

        public string GeminiModel
        {
            get => GetSetting(GeminiModelKey, DefaultGeminiModel);
            set => SetSetting(GeminiModelKey, value);
        }

        #endregion

        #region Ollama Settings

        public string OllamaBaseUrl
        {
            get => GetSetting(OllamaBaseUrlKey, DefaultOllamaBaseUrl);
            set => SetSetting(OllamaBaseUrlKey, value);
        }

        public string OllamaModel
        {
            get => GetSetting(OllamaModelKey, DefaultOllamaModel);
            set => SetSetting(OllamaModelKey, value);
        }

        #endregion

        #region Helper Methods

        private string GetSetting(string key, string defaultValue)
        {
            if (_localSettings.Values.TryGetValue(key, out var value) && value is string strValue)
            {
                return strValue;
            }
            return defaultValue;
        }

        private void SetSetting(string key, string value)
        {
            _localSettings.Values[key] = value;
        }

        #endregion
    }
}
