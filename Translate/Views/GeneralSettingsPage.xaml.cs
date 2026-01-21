using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Translate.Services;

namespace Translate.Views;

/// <summary>
/// General settings page for configuring application-wide settings.
/// </summary>
public sealed partial class GeneralSettingsPage : Page
{
    private readonly SettingsService _settings;

    public int MaxHistoryCount { get; set; }

    public GeneralSettingsPage()
    {
        _settings = App.Current.Services.GetRequiredService<SettingsService>();
        MaxHistoryCount = _settings.MaxHistoryCount;
        InitializeComponent();
    }

    private void MaxHistoryNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (!double.IsNaN(args.NewValue))
        {
            _settings.MaxHistoryCount = (int)args.NewValue;
        }
    }
}
