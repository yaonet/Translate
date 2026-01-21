using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT.Interop;

namespace Translate.Views;

/// <summary>
/// Settings window with NavigationView for configuring application settings.
/// </summary>
public sealed partial class SettingWindow : Window
{
    public SettingWindow()
    {
        InitializeComponent();

        // Use custom TitleBar instead of native window title bar
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        // Set window size
        var hwnd = WindowNative.GetWindowHandle(this);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        appWindow.Resize(new Windows.Graphics.SizeInt32(1000, 800));

        // Select the first item by default
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem selectedItem)
        {
            var tag = selectedItem.Tag?.ToString();

            switch (tag)
            {
                case "History":
                    ContentFrame.Navigate(typeof(HistoryPage));
                    break;
                case "General":
                    ContentFrame.Navigate(typeof(GeneralSettingsPage));
                    break;
                case "Gemini":
                    ContentFrame.Navigate(typeof(GeminiSettingsPage));
                    break;
                case "Ollama":
                    ContentFrame.Navigate(typeof(OllamaSettingsPage));
                    break;
                case "DeepSeek":
                    ContentFrame.Navigate(typeof(DeepSeekSettingsPage));
                    break;
                case "OpenAI":
                    ContentFrame.Navigate(typeof(OpenAISettingsPage));
                    break;
                case "About":
                    ContentFrame.Navigate(typeof(AboutPage));
                    break;
            }
        }
    }

    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }
}
