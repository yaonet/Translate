using CommunityToolkit.Mvvm.ComponentModel;
using Translate.Services;

namespace Translate.ViewModels
{
    public partial class SettingViewModel : ObservableObject
    {
        private readonly SettingsService _settings;

        public SettingViewModel(SettingsService settings)
        {
            _settings = settings;
            // Load current values from settings
            GeminiApiKey = _settings.GeminiApiKey;
            GeminiBaseUrl = _settings.GeminiBaseUrl;
            GeminiModel = _settings.GeminiModel;
            OllamaBaseUrl = _settings.OllamaBaseUrl;
            OllamaModel = _settings.OllamaModel;
            DeepSeekApiKey = _settings.DeepSeekApiKey;
            DeepSeekBaseUrl = _settings.DeepSeekBaseUrl;
            DeepSeekModel = _settings.DeepSeekModel;
            OpenAIApiKey = _settings.OpenAIApiKey;
            OpenAIBaseUrl = _settings.OpenAIBaseUrl;
            OpenAIModel = _settings.OpenAIModel;
        }

        #region Gemini Settings

        [ObservableProperty]
        public partial string GeminiApiKey { get; set; }

        partial void OnGeminiApiKeyChanged(string value)
        {
            _settings.GeminiApiKey = value;
        }

        [ObservableProperty]
        public partial string GeminiBaseUrl { get; set; }

        partial void OnGeminiBaseUrlChanged(string value)
        {
            _settings.GeminiBaseUrl = value;
        }

        [ObservableProperty]
        public partial string GeminiModel { get; set; }

        partial void OnGeminiModelChanged(string value)
        {
            _settings.GeminiModel = value;
        }

        #endregion

        #region Ollama Settings

        [ObservableProperty]
        public partial string OllamaBaseUrl { get; set; }

        partial void OnOllamaBaseUrlChanged(string value)
        {
            _settings.OllamaBaseUrl = value;
        }

        [ObservableProperty]
        public partial string OllamaModel { get; set; }

        partial void OnOllamaModelChanged(string value)
        {
            _settings.OllamaModel = value;
        }

        #endregion

        #region DeepSeek Settings

        [ObservableProperty]
        public partial string DeepSeekApiKey { get; set; }

        partial void OnDeepSeekApiKeyChanged(string value)
        {
            _settings.DeepSeekApiKey = value;
        }

        [ObservableProperty]
        public partial string DeepSeekBaseUrl { get; set; }

        partial void OnDeepSeekBaseUrlChanged(string value)
        {
            _settings.DeepSeekBaseUrl = value;
        }

        [ObservableProperty]
        public partial string DeepSeekModel { get; set; }

        partial void OnDeepSeekModelChanged(string value)
        {
            _settings.DeepSeekModel = value;
        }

        #endregion

        #region OpenAI Settings

        [ObservableProperty]
        public partial string OpenAIApiKey { get; set; }

        partial void OnOpenAIApiKeyChanged(string value)
        {
            _settings.OpenAIApiKey = value;
        }

        [ObservableProperty]
        public partial string OpenAIBaseUrl { get; set; }

        partial void OnOpenAIBaseUrlChanged(string value)
        {
            _settings.OpenAIBaseUrl = value;
        }

        [ObservableProperty]
        public partial string OpenAIModel { get; set; }

        partial void OnOpenAIModelChanged(string value)
        {
            _settings.OpenAIModel = value;
        }

        #endregion
    }
}
