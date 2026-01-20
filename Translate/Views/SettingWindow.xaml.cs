using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Translate.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Translate.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingWindow : Window
{
    private readonly SettingViewModel _viewModel = default!;
    public SettingWindow()
    {
        InitializeComponent();
        _viewModel = App.Current.Services.GetRequiredService<SettingViewModel>();
    }
}
