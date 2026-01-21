using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Translate.Services;
using System;
using Windows.Storage;
using Windows.System;

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

    private async void OpenLogFolder_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            var logPath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "logs");
            if (System.IO.Directory.Exists(logPath))
            {
                var folder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(logPath);
                await Windows.System.Launcher.LaunchFolderAsync(folder);
            }
            else
            {
                await Windows.System.Launcher.LaunchFolderAsync(Windows.Storage.ApplicationData.Current.LocalFolder);
            }
        }
        catch
        {
            // Ignore errors, potentially fallback to just opening local folder if specific path fails
            try
            {
                await Windows.System.Launcher.LaunchFolderAsync(Windows.Storage.ApplicationData.Current.LocalFolder);
            }
            catch { }
        }
    }
}
