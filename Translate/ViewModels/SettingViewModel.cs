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
            GeminiModel = _settings.GeminiModel;
            OllamaBaseUrl = _settings.OllamaBaseUrl;
            OllamaModel = _settings.OllamaModel;
        }

        #region Gemini Settings

        [ObservableProperty]
        public partial string GeminiApiKey { get; set; }

        partial void OnGeminiApiKeyChanged(string value)
        {
            _settings.GeminiApiKey = value;
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
    }
}
