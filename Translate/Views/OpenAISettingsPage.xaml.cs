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
    private readonly SettingViewModel _viewModel;

    public OpenAISettingsPage()
    {
        InitializeComponent();
        _viewModel = App.Current.Services.GetRequiredService<SettingViewModel>();
        
        // Initialize controls with current values
        OpenAIApiKeyBox.Password = _viewModel.OpenAIApiKey;
        OpenAIBaseUrlTextBox.Text = _viewModel.OpenAIBaseUrl;
        OpenAIModelTextBox.Text = _viewModel.OpenAIModel;

        // Subscribe to TextChanged events for two-way binding
        OpenAIBaseUrlTextBox.TextChanged += (s, e) => _viewModel.OpenAIBaseUrl = OpenAIBaseUrlTextBox.Text;
        OpenAIModelTextBox.TextChanged += (s, e) => _viewModel.OpenAIModel = OpenAIModelTextBox.Text;
    }

    private void OpenAIApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            _viewModel.OpenAIApiKey = passwordBox.Password;
        }
    }
}
