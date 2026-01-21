using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Translate.Data;
using Translate.Services;
using Translate.ViewModels;
using Windows.Storage;

namespace Translate
{
    public partial class App : Application
    {
        private MainWindow? _window;
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddSingleton<HttpClient>();
            services.AddSingleton<SettingsService>();
            services.AddSingleton(new TextSelectionService());
            services.AddSingleton<IWindowService, WindowService>();

            // Keyed translate services
            var translateServiceKeys = new List<string>();

            services.AddKeyedSingleton<ITranslateService, OllamaTranslateService>("Ollama");
            translateServiceKeys.Add("Ollama");

            services.AddKeyedSingleton<ITranslateService, GeminiTranslateService>("Gemini");
            translateServiceKeys.Add("Gemini");

            services.AddKeyedSingleton<ITranslateService, DeepSeekTranslateService>("DeepSeek");
            translateServiceKeys.Add("DeepSeek");

            services.AddKeyedSingleton<ITranslateService, OpenAITranslateService>("OpenAI");
            translateServiceKeys.Add("OpenAI");

            // Register the list of available translate service keys
            services.AddSingleton<IReadOnlyList<string>>(translateServiceKeys);

            // Database context for translation history
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "translate_history.db");
            services.AddDbContextFactory<TranslateDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // History service
            services.AddSingleton<HistoryService>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<SettingViewModel>();
            services.AddTransient<HistoryViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
