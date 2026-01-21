using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Translate.ViewModels;

namespace Translate.Views;

/// <summary>
/// Settings page for DeepSeek translation service configuration.
/// </summary>
public sealed partial class DeepSeekSettingsPage : Page
{
    public SettingViewModel ViewModel { get; }

    public DeepSeekSettingsPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<SettingViewModel>();
        InitializeComponent();

        // Initialize controls with current valuese (PasswordBox doesn't support direct binding in WinUI)
        DeepSeekApiKeyBox.Password = ViewModel.DeepSeekApiKey;
    }

    private void DeepSeekApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            ViewModel.DeepSeekApiKey = passwordBox.Password;
        }
    }
}
