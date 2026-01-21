using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Translate.Models;
using Translate.Services;

namespace Translate.ViewModels
{
    public partial class HistoryViewModel : ObservableObject
    {
        private readonly HistoryService _historyService;

        public HistoryViewModel(HistoryService historyService)
        {
            _historyService = historyService;
            HistoryItems = new ObservableCollection<TranslationHistoryItem>();
        }

        public ObservableCollection<TranslationHistoryItem> HistoryItems { get; }

        [ObservableProperty]
        public partial bool IsLoading { get; set; }

        [RelayCommand]
        private async Task LoadHistoryAsync()
        {
            IsLoading = true;
            try
            {
                var items = await _historyService.GetAllAsync();
                HistoryItems.Clear();
                foreach (var item in items)
                {
                    HistoryItems.Add(item);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteItemAsync(TranslationHistoryItem? item)
        {
            if (item == null) return;

            await _historyService.DeleteAsync(item.Id);
            HistoryItems.Remove(item);
        }

        [RelayCommand]
        private async Task ClearAllAsync()
        {
            await _historyService.ClearAllAsync();
            HistoryItems.Clear();
        }
    }
}
