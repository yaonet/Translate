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
    private readonly SettingViewModel _viewModel;

    public DeepSeekSettingsPage()
    {
        InitializeComponent();
        _viewModel = App.Current.Services.GetRequiredService<SettingViewModel>();
        
        // Initialize controls with current values
        DeepSeekApiKeyBox.Password = _viewModel.DeepSeekApiKey;
        DeepSeekBaseUrlTextBox.Text = _viewModel.DeepSeekBaseUrl;
        DeepSeekModelTextBox.Text = _viewModel.DeepSeekModel;

        // Subscribe to TextChanged events for two-way binding
        DeepSeekBaseUrlTextBox.TextChanged += (s, e) => _viewModel.DeepSeekBaseUrl = DeepSeekBaseUrlTextBox.Text;
        DeepSeekModelTextBox.TextChanged += (s, e) => _viewModel.DeepSeekModel = DeepSeekModelTextBox.Text;
    }

    private void DeepSeekApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            _viewModel.DeepSeekApiKey = passwordBox.Password;
        }
    }
}
