using CommunityToolkit.Mvvm.ComponentModel;
using Translate.Services;

namespace Translate.ViewModels
{
    public partial class SettingViewModel(SettingsService settings) : ObservableObject
    {
        private readonly SettingsService _settings = settings;
    }
}
