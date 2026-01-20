using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Translate.ViewModels;
using WinRT.Interop;

namespace Translate.Views;

/// <summary>
/// Settings window for configuring translation services.
/// </summary>
public sealed partial class SettingWindow : Window
{
    private readonly SettingViewModel _viewModel;

    public SettingWindow()
    {
        InitializeComponent();
        _viewModel = App.Current.Services.GetRequiredService<SettingViewModel>();

        // Set window size
        var hwnd = WindowNative.GetWindowHandle(this);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        appWindow.Resize(new Windows.Graphics.SizeInt32(400, 450));

        // Initialize PasswordBox with current API key
        GeminiApiKeyBox.Password = _viewModel.GeminiApiKey;
    }

    private void GeminiApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            _viewModel.GeminiApiKey = passwordBox.Password;
        }
    }
}
