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
    public SettingViewModel ViewModel { get; }

    public GeminiSettingsPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<SettingViewModel>();
        InitializeComponent();

        // Initialize PasswordBox with current value (PasswordBox doesn't support direct binding in WinUI)
        GeminiApiKeyBox.Password = ViewModel.GeminiApiKey;
    }

    private void GeminiApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            ViewModel.GeminiApiKey = passwordBox.Password;
        }
    }
}
