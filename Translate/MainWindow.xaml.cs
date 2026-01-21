using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Translate.Services;
using Translate.ViewModels;
using System.ComponentModel;

namespace Translate
{
    public sealed partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel = default!;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = App.Current.Services.GetRequiredService<MainViewModel>();

            // Inject DispatcherQueue for UI thread marshalling
            var textSelectionService = App.Current.Services.GetRequiredService<TextSelectionService>();
            textSelectionService.SetDispatcherQueue(DispatcherQueue);

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsMaximizable = false;
                presenter.IsMinimizable = false;
                presenter.IsAlwaysOnTop = true;
            }

            var windowSize = new Windows.Graphics.SizeInt32(660, 500);
            AppWindow.Resize(windowSize);
            var displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Primary);
            var centerX = (displayArea.WorkArea.Width - windowSize.Width) / 2;
            var centerY = (displayArea.WorkArea.Height - windowSize.Height) / 2;
            AppWindow.Move(new Windows.Graphics.PointInt32(centerX, centerY));

            // Subscribe to ViewModel property changes for AlwaysOnTop
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Listen for the window Closed event to cleanup resources.
            Closed += (_, _) =>
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.Cleanup();
            };
        }
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.IsAlwaysOnTop) &&
                AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsAlwaysOnTop = _viewModel.IsAlwaysOnTop;
            }
            else if (e.PropertyName == nameof(MainViewModel.CurrentThemeIndex))
            {
                ApplyTheme(_viewModel.CurrentThemeIndex);
            }
        }

        private void ApplyTheme(int themeIndex)
        {
            if (Content is FrameworkElement rootElement)
            {
                var theme = themeIndex switch
                {
                    1 => ElementTheme.Light,
                    2 => ElementTheme.Dark,
                    _ => ElementTheme.Default
                };
                rootElement.RequestedTheme = theme;
                UpdateTitleBar(theme);
            }
        }

        private void UpdateTitleBar(ElementTheme theme)
        {
            if (AppWindow.TitleBar == null) return;

            var titleBar = AppWindow.TitleBar;

            var computedTheme = theme;
            if (computedTheme == ElementTheme.Default)
            {
                computedTheme = Application.Current.RequestedTheme == ApplicationTheme.Dark
                    ? ElementTheme.Dark
                    : ElementTheme.Light;
            }

            if (computedTheme == ElementTheme.Dark)
            {
                titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
                titleBar.ButtonForegroundColor = Microsoft.UI.Colors.White;
                titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Microsoft.UI.Colors.LightGray;
                titleBar.ButtonHoverBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(32, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.White;
                titleBar.ButtonPressedBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(64, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.White;
            }
            else
            {
                titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
                titleBar.ButtonForegroundColor = Microsoft.UI.Colors.Black;
                titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Microsoft.UI.Colors.DarkGray;
                titleBar.ButtonHoverBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(32, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(64, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.Black;
            }
        }
    }
}
