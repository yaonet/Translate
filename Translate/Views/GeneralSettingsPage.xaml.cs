using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Translate.ViewModels;

namespace Translate.Views;

/// <summary>
/// General settings page for configuring application-wide settings.
/// </summary>
public sealed partial class GeneralSettingsPage : Page
{
    public GeneralSettingsViewModel ViewModel { get; }

    public GeneralSettingsPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<GeneralSettingsViewModel>();
        InitializeComponent();
    }
}
