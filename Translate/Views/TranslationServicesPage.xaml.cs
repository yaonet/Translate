using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Translate.ViewModels;

namespace Translate.Views;

/// <summary>
/// Translation services settings page for Gemini and Ollama configuration.
/// </summary>
public sealed partial class TranslationServicesPage : Page
{
    private readonly SettingViewModel _viewModel;

    public TranslationServicesPage()
    {
        InitializeComponent();
        _viewModel = App.Current.Services.GetRequiredService<SettingViewModel>();
        
        // Initialize controls with current values
        GeminiApiKeyBox.Password = _viewModel.GeminiApiKey;
        GeminiModelTextBox.Text = _viewModel.GeminiModel;
        OllamaBaseUrlTextBox.Text = _viewModel.OllamaBaseUrl;
        OllamaModelTextBox.Text = _viewModel.OllamaModel;

        // Subscribe to TextChanged events for two-way binding
        GeminiModelTextBox.TextChanged += (s, e) => _viewModel.GeminiModel = GeminiModelTextBox.Text;
        OllamaBaseUrlTextBox.TextChanged += (s, e) => _viewModel.OllamaBaseUrl = OllamaBaseUrlTextBox.Text;
        OllamaModelTextBox.TextChanged += (s, e) => _viewModel.OllamaModel = OllamaModelTextBox.Text;
    }

    private void GeminiApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            _viewModel.GeminiApiKey = passwordBox.Password;
        }
    }
}
