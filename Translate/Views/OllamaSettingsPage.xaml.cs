using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Translate.ViewModels;

namespace Translate.Views;

/// <summary>
/// Settings page for Ollama translation service configuration.
/// </summary>
public sealed partial class OllamaSettingsPage : Page
{
    public SettingViewModel ViewModel { get; }

    public OllamaSettingsPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<SettingViewModel>();
        InitializeComponent();
    }
}
