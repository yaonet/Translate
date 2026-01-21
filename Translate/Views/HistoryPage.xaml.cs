using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Translate.Models;
using Translate.ViewModels;

namespace Translate.Views;

/// <summary>
/// History page displaying translation history items.
/// </summary>
public sealed partial class HistoryPage : Page
{
    public HistoryViewModel ViewModel { get; }

    public HistoryPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<HistoryViewModel>();
        InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadHistoryCommand.ExecuteAsync(null);
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        // Note: DataTemplate bindings scope to the data item, so we use Tag to pass 
        // the item to the ViewModel command. This is an acceptable MVVM pattern for WinUI.
        if (sender is Button button && button.Tag is TranslationHistoryItem item)
        {
            await ViewModel.DeleteItemCommand.ExecuteAsync(item);
        }
    }
}
