using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Translate.ViewModels;

namespace Translate.Views;

/// <summary>
/// Settings page for Gemini translation service configuration.
/// </summary>
public sealed partial class GeminiSettingsPage : Page
{
    private readonly SettingViewModel _viewModel;

    public GeminiSettingsPage()
    {
        InitializeComponent();
        _viewModel = App.Current.Services.GetRequiredService<SettingViewModel>();
        
        // Initialize controls with current values
        GeminiApiKeyBox.Password = _viewModel.GeminiApiKey;
        GeminiModelTextBox.Text = _viewModel.GeminiModel;

        // Subscribe to TextChanged event for two-way binding
        GeminiModelTextBox.TextChanged += (s, e) => _viewModel.GeminiModel = GeminiModelTextBox.Text;
    }

    private void GeminiApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            _viewModel.GeminiApiKey = passwordBox.Password;
        }
    }
}
