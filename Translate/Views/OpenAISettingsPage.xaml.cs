using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Translate.ViewModels;

namespace Translate.Views;

/// <summary>
/// Settings page for OpenAI translation service configuration.
/// </summary>
public sealed partial class OpenAISettingsPage : Page
{
    public SettingViewModel ViewModel { get; }

    public OpenAISettingsPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<SettingViewModel>();
        InitializeComponent();

        // Initialize controls with current valuese (PasswordBox doesn't support direct binding in WinUI)
        OpenAIApiKeyBox.Password = ViewModel.OpenAIApiKey;
    }

    private void OpenAIApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            ViewModel.OpenAIApiKey = passwordBox.Password;
        }
    }
}
