using Windows.Security.Credentials;
using Windows.Storage;

namespace Translate.Services
{
    /// <summary>
    /// Service for managing application settings using LocalSettings and PasswordVault for secure storage.
    /// </summary>
    public class SettingsService
    {
        private readonly ApplicationDataContainer _localSettings;
        private readonly PasswordVault _vault;

        // Setting keys
        private const string GeminiApiKeyKey = "GeminiApiKey";
        private const string GeminiBaseUrlKey = "GeminiBaseUrl";
        private const string GeminiModelKey = "GeminiModel";
        private const string OllamaBaseUrlKey = "OllamaBaseUrl";
        private const string OllamaModelKey = "OllamaModel";
        private const string DeepSeekApiKeyKey = "DeepSeekApiKey";
        private const string DeepSeekBaseUrlKey = "DeepSeekBaseUrl";
        private const string DeepSeekModelKey = "DeepSeekModel";
        private const string OpenAIApiKeyKey = "OpenAIApiKey";
        private const string OpenAIBaseUrlKey = "OpenAIBaseUrl";
        private const string OpenAIModelKey = "OpenAIModel";
        private const string MaxHistoryCountKey = "MaxHistoryCount";

        // Resource name for PasswordVault
        private const string VaultResource = "TranslateApp";

        // Default values
        private const string DefaultGeminiBaseUrl = "https://generativelanguage.googleapis.com";
        private const string DefaultGeminiModel = "gemini-flash-latest";
        private const string DefaultOllamaBaseUrl = "http://localhost:11434";
        private const string DefaultOllamaModel = "translategemma";
        private const string DefaultDeepSeekBaseUrl = "https://api.deepseek.com";
        private const string DefaultDeepSeekModel = "deepseek-chat";
        private const string DefaultOpenAIBaseUrl = "https://api.openai.com/v1";
        private const string DefaultOpenAIModel = "gpt-4o-mini";
        private const int DefaultMaxHistoryCount = 300;

        public SettingsService()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
            _vault = new PasswordVault();
        }

        #region Gemini Settings

        public string GeminiApiKey
        {
            get => GetSecureSetting(GeminiApiKeyKey);
            set => SetSecureSetting(GeminiApiKeyKey, value);
        }

        public string GeminiBaseUrl
        {
            get => GetSetting(GeminiBaseUrlKey, DefaultGeminiBaseUrl);
            set => SetSetting(GeminiBaseUrlKey, value);
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

        #region DeepSeek Settings

        public string DeepSeekApiKey
        {
            get => GetSecureSetting(DeepSeekApiKeyKey);
            set => SetSecureSetting(DeepSeekApiKeyKey, value);
        }

        public string DeepSeekBaseUrl
        {
            get => GetSetting(DeepSeekBaseUrlKey, DefaultDeepSeekBaseUrl);
            set => SetSetting(DeepSeekBaseUrlKey, value);
        }

        public string DeepSeekModel
        {
            get => GetSetting(DeepSeekModelKey, DefaultDeepSeekModel);
            set => SetSetting(DeepSeekModelKey, value);
        }

        #endregion

        #region OpenAI Settings

        public string OpenAIApiKey
        {
            get => GetSecureSetting(OpenAIApiKeyKey);
            set => SetSecureSetting(OpenAIApiKeyKey, value);
        }

        public string OpenAIBaseUrl
        {
            get => GetSetting(OpenAIBaseUrlKey, DefaultOpenAIBaseUrl);
            set => SetSetting(OpenAIBaseUrlKey, value);
        }

        public string OpenAIModel
        {
            get => GetSetting(OpenAIModelKey, DefaultOpenAIModel);
            set => SetSetting(OpenAIModelKey, value);
        }

        #endregion

        #region General Settings

        public int MaxHistoryCount
        {
            get => GetIntSetting(MaxHistoryCountKey, DefaultMaxHistoryCount);
            set => SetIntSetting(MaxHistoryCountKey, value);
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

        private int GetIntSetting(string key, int defaultValue)
        {
            if (_localSettings.Values.TryGetValue(key, out var value) && value is int intValue)
            {
                return intValue;
            }
            return defaultValue;
        }

        private void SetIntSetting(string key, int value)
        {
            _localSettings.Values[key] = value;
        }

        /// <summary>
        /// Retrieves a secure setting from the Windows PasswordVault.
        /// </summary>
        private string GetSecureSetting(string key)
        {
            try
            {
                var credential = _vault.Retrieve(VaultResource, key);
                credential.RetrievePassword();
                return credential.Password;
            }
            catch
            {
                // Credential not found or access denied
                return string.Empty;
            }
        }

        /// <summary>
        /// Stores a secure setting in the Windows PasswordVault.
        /// </summary>
        private void SetSecureSetting(string key, string value)
        {
            try
            {
                // Remove existing credential if present
                var existing = _vault.Retrieve(VaultResource, key);
                _vault.Remove(existing);
            }
            catch
            {
                // Credential doesn't exist, ignore
            }

            if (!string.IsNullOrEmpty(value))
            {
                _vault.Add(new PasswordCredential(VaultResource, key, value));
            }
        }

        #endregion
    }
}

