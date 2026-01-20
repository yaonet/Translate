using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Text;
using Translate.Services;

namespace Translate.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ITranslateService _translateService;
    private readonly TextSelectionService _textSelectionService;

    public MainViewModel(ITranslateService translateService, TextSelectionService textSelection)
    {
        _translateService = translateService;
        _textSelectionService = textSelection;
        _textSelectionService.TextSelected += OnTextSelected;
    }

    [ObservableProperty]
    public partial bool IsAutoTranslate { get; set; }

    [ObservableProperty]
    public partial bool IsAlwaysOnTop { get; set; } = true;

    // Theme: 0 = System, 1 = Light, 2 = Dark
    [ObservableProperty]
    public partial int CurrentThemeIndex { get; set; } = 0;

    // Icons: E793 = Sun (Light), E708 = Moon (Dark), E771 = Settings (System)
    public string ThemeGlyph => CurrentThemeIndex switch
    {
        1 => "\uE706",  // Sunny - Light mode
        2 => "\uE708",  // Moon - Dark mode
        _ => "\uEC8A"   // System default
    };

    [ObservableProperty]
    public partial string SourceText { get; set; } = "Enter text to translate...";

    [ObservableProperty]
    public partial string TargetText { get; set; } = "Translated text will appear here...";

    [ObservableProperty]
    public partial string SourceCode { get; set; } = "en";

    [ObservableProperty]
    public partial string TargetCode { get; set; } = "zh";

    public string[] Code { get; } = ["zh", "en", "ja", "ko"];
    partial void OnIsAutoTranslateChanged(bool value)
    {
        if (value)
            _textSelectionService?.Start();
        else
            _textSelectionService?.Stop();
    }
    private async void OnTextSelected(string text)
    {
        SourceText = text;
        await DoTranslateAsync();
    }

    [RelayCommand]
    private async Task DoTranslateAsync()
    {
        if (string.IsNullOrWhiteSpace(SourceText) ||
            SourceText == "Enter text to translate...")
        {
            TargetText = "Please enter text to translate.";
            return;
        }

        TargetText = "Translating...";

        var request = new TranslationRequest(SourceText, SourceCode, TargetCode);
        var result = await _translateService.TranslateAsync(request);

        TargetText = result.Success
            ? result.TranslatedText!
            : result.ErrorMessage!;
    }

    [RelayCommand]
    private void SwitchLanguage()
    {
        (TargetCode, SourceCode) = (SourceCode, TargetCode);
    }

    [RelayCommand]
    private void SwitchText()
    {
        (TargetText, SourceText) = (SourceText, TargetText);
    }

    [RelayCommand]
    private void SwitchTheme()
    {
        CurrentThemeIndex = (CurrentThemeIndex + 1) % 3;
    }

    partial void OnCurrentThemeIndexChanged(int value)
    {
        OnPropertyChanged(nameof(ThemeGlyph));
    }

    //为什么不使用IDisposable ？
    //viewModel由依赖注入管理，还需要手动释放吗？
    public void Cleanup()
    {
        if (_textSelectionService != null)
        {
            _textSelectionService.TextSelected -= OnTextSelected;
        }
    }
}
