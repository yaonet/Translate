using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Translate
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly HttpClient _http;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsAutoTranslateEnabled
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged();
                    if (value)
                    {
                        App.Instance.StartMouseHook();
                    }
                    else
                    {
                        App.Instance.StopMouseHook();
                    }
                }
            }
        } = false;

        public bool IsAlwaysOnTop
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged();
                    if (AppWindow.Presenter is OverlappedPresenter presenter)
                    {
                        presenter.IsAlwaysOnTop = value;
                    }
                }
            }
        } = true;

        public ElementTheme CurrentTheme
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ThemeIcon));
                    if (Content is FrameworkElement root)
                    {
                        root.RequestedTheme = value;
                    }
                }
            }
        } = ElementTheme.Default;

        public string ThemeIcon => CurrentTheme switch
        {
            ElementTheme.Light => "\uE708",
            ElementTheme.Dark => "\uE706",
            _ => "\uE793"
        };

        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsMaximizable = false;
                presenter.IsMinimizable = false;
                presenter.IsAlwaysOnTop = IsAlwaysOnTop;
            }

            var windowSize = new Windows.Graphics.SizeInt32(600, 500);
            AppWindow.Resize(windowSize);
            var displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Primary);
            var centerX = (displayArea.WorkArea.Width - windowSize.Width) / 2;
            var centerY = (displayArea.WorkArea.Height - windowSize.Height) / 2;
            AppWindow.Move(new Windows.Graphics.PointInt32(centerX, centerY));

            _http = new HttpClient();

            App.Instance.TextSelected += OnTextSelected;

            Closed += (_, _) =>
            {
                App.Instance.TextSelected -= OnTextSelected;
                _http?.Dispose();
            };
        }

        private async void OnTextSelected(string text)
        {
            SourceText = text;
            await TranslateTextAsync();
        }
        public string SourceText 
        { 
            get; 
            set
            {
                if(field != value)
                {
                    field = value;
                    OnPropertyChanged();
                }
            }
        } = "Enter text to translate...";
        public string TargetText
        {
            get => field;
            set
            {
                if(field != value)
                {
                    field = value;
                    OnPropertyChanged();
                }
            }
        } = "Translated text will appear here...";
        public string SourceCode 
        { 
            get; 
            set
            {
                if(field!=value)
                {
                    field = value;
                    OnPropertyChanged();
                }
            }
        } = "en";
        public string TargetCode 
        { 
            get; 
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged();
                }
            }
        } = "zh";
        public string[] Code { get; } = ["zh", "en", "ja", "ko"];

        static string CodeToLanguage(string code)
        {
            return code switch
            {
                "en" => "English",
                "zh" => "Chinese",
                "ja" => "Japanese",
                "ko" => "Korean",
                _ => code
            };
        }
        private async void DoTranslate(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = false;
            }

            try
            {
                await TranslateTextAsync();
            }
            finally
            {
                if (sender is Button b)
                {
                    b.IsEnabled = true;
                }
            }
        }

        private async Task TranslateTextAsync()
        {
            if (string.IsNullOrWhiteSpace(SourceText) ||
                SourceText == "Enter text to translate...")
            {
                TargetText = "Please enter text to translate.";
                return;
            }

            TargetText = "Translating...";

            try
            {
                string sourceLanguage = CodeToLanguage(SourceCode);
                string targetLanguage = CodeToLanguage(TargetCode);

                string prompt = $$"""
                    You are a professional {{sourceLanguage}} ({{SourceCode}}) to {{targetLanguage}} ({{TargetCode}}) translator. Your goal is to accurately convey the meaning and nuances of the original {{sourceLanguage}} text while adhering to {{targetLanguage}} grammar, vocabulary, and cultural sensitivities.
                    Produce only the {{targetLanguage}} translation, without any additional explanations or commentary. Please translate the following {{sourceLanguage}} text into {{targetLanguage}}:

                    {{SourceText}}
                    """;

                var body = new
                {
                    model = "translategemma",
                    prompt = prompt!,
                    stream = false
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await _http.PostAsync("http://localhost:11434/api/generate", content);
                if (!response.IsSuccessStatusCode)
                {
                    TargetText = $"Translation failed: {response.StatusCode}";
                    return;
                }

                var result = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(result);

                var translation = doc.RootElement.GetProperty("response").GetString();
                TargetText = string.IsNullOrWhiteSpace(translation)
                    ? "No translation returned."
                    : translation;
            }
            catch (HttpRequestException ex)
            {
                TargetText = $"Network error: {ex.Message}";
            }
            catch (JsonException ex)
            {
                TargetText = $"Invalid response format: {ex.Message}";
            }
            catch (Exception ex)
            {
                TargetText = $"Error: {ex.Message}";
            }
        }
        private void SwitchLanguage(object sender, RoutedEventArgs e)
        {
            (TargetCode, SourceCode) = (SourceCode, TargetCode);
        }

        private void SwitchText(object sender, RoutedEventArgs e)
        {
            (TargetText, SourceText) = (SourceText, TargetText);
        }

        private void ToggleTheme(object sender, RoutedEventArgs e)
        {
            CurrentTheme = CurrentTheme switch
            {
                ElementTheme.Light => ElementTheme.Dark,
                ElementTheme.Dark => ElementTheme.Light,
                ElementTheme.Default => ElementTheme.Dark,
                _ => ElementTheme.Default
            };
        }
    }
}
