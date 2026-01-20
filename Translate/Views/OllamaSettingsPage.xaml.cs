using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Translate.ViewModels;

namespace Translate.Views;

/// <summary>
/// Settings page for Ollama translation service configuration.
/// </summary>
public sealed partial class OllamaSettingsPage : Page
{
    private readonly SettingViewModel _viewModel;

    public OllamaSettingsPage()
    {
        InitializeComponent();
        _viewModel = App.Current.Services.GetRequiredService<SettingViewModel>();
        
        // Initialize controls with current values
        OllamaBaseUrlTextBox.Text = _viewModel.OllamaBaseUrl;
        OllamaModelTextBox.Text = _viewModel.OllamaModel;

        // Subscribe to TextChanged events for two-way binding
        OllamaBaseUrlTextBox.TextChanged += (s, e) => _viewModel.OllamaBaseUrl = OllamaBaseUrlTextBox.Text;
        OllamaModelTextBox.TextChanged += (s, e) => _viewModel.OllamaModel = OllamaModelTextBox.Text;
    }
}
