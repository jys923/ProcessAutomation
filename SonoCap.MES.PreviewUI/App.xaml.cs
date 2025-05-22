using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SonoCap.Commons;
using SonoCap.MES.PreviewUI.ViewModels;
using SonoCap.MES.PreviewUI.Views;
using SonoCap.MES.Services;
using SonoCap.MES.Services.Interfaces;
using SonoCap.MES.Services.Model;
using System.IO;
using System.Windows;

namespace SonoCap.MES.PreviewUI
{
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; private set; }
        public static AppSettings appSettings { get; set; } = new AppSettings();

        public App()
        {
            ConfigureLogging();
            Services = ConfigureServices();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var view = new PreviewSaveView();
            view.DataContext = Services.GetRequiredService<PreviewSaveViewModel>();

            // ✅ 직접 ViewService-like 핸들러 호출
            if (view.DataContext is PreviewSaveViewModel vm)
            {
                vm.SetWindow(view); // 이 안에서 AddLifecycleHandler 호출됨
            }

            view.Show();
        }

        private void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("previewui.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private IServiceProvider ConfigureServices()
        {
            IConfiguration configuration = ConfigureAppSettings();
            configuration.Bind(appSettings);

            LoggingConfigurator.Configure(appSettings.Serilog); // ← 이렇게 바뀜

            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<GlobalModel>();
            services.AddTransient<ISerialPortWrapper, SerialPortWrapper>();
            services.AddSingleton<IMotorService, MotorService>();

            services.AddTransient<PreviewSaveViewModel>();
            services.AddTransient<PreviewSaveView>();

            return services.BuildServiceProvider();
        }

        private static IConfiguration ConfigureAppSettings()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }
    }
}
