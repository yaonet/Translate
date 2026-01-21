using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Translate.Services;
using Windows.Storage;
using Windows.System;

namespace Translate.ViewModels;

/// <summary>
/// ViewModel for general application settings.
/// </summary>
public partial class GeneralSettingsViewModel : ObservableObject
{
    private readonly SettingsService _settings;

    public GeneralSettingsViewModel(SettingsService settings)
    {
        _settings = settings;
        MaxHistoryCount = _settings.MaxHistoryCount;
    }

    [ObservableProperty]
    public partial int MaxHistoryCount { get; set; }

    partial void OnMaxHistoryCountChanged(int value)
    {
        _settings.MaxHistoryCount = value;
    }

    [RelayCommand]
    private static async Task OpenLogFolderAsync()
    {
        try
        {
            var logPath = System.IO.Path.Combine(
                ApplicationData.Current.LocalFolder.Path, "logs");

            if (System.IO.Directory.Exists(logPath))
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(logPath).AsTask();
                await Launcher.LaunchFolderAsync(folder).AsTask();
            }
            else
            {
                await Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder).AsTask();
            }
        }
        catch
        {
            // Fallback to opening local folder if specific path fails
            try
            {
                await Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder).AsTask();
            }
            catch { }
        }
    }
}
