using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Serilog;
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
        private Microsoft.Extensions.Logging.ILogger<App>? _logger;

        public App()
        {
            // Global exception handling
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            Services = ConfigureServices();
            InitializeComponent();

            _logger = Services.GetService<Microsoft.Extensions.Logging.ILogger<App>>();
        }

        private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            _logger?.LogCritical(e.ExceptionObject as Exception, "Unhandled AppDomain exception occurred.");
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            _logger?.LogError(e.Exception, "Unobserved Task exception occurred.");
            e.SetObserved();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Configure Serilog
            string logPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "logs", "log-.txt");
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(dispose: true);
            });

            // Services
            services.AddSingleton<HttpClient>();
            services.AddSingleton<SettingsService>();
            services.AddSingleton<TextSelectionService>();
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
            services.AddTransient<GeneralSettingsViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
