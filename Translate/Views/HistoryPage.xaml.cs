using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Translate.Models;
using Translate.ViewModels;

namespace Translate.Views
{
    public sealed partial class HistoryPage : Page
    {
        private readonly HistoryViewModel _viewModel;

        public HistoryPage()
        {
            InitializeComponent();
            _viewModel = App.Current.Services.GetRequiredService<HistoryViewModel>();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadHistoryCommand.ExecuteAsync(null);
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TranslationHistoryItem item)
            {
                await _viewModel.DeleteItemCommand.ExecuteAsync(item);
            }
        }
    }
}
