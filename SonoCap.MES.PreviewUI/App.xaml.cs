using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SonoCap.MES.PreviewUI.ViewModels;
using SonoCap.MES.PreviewUI.Views;
using SonoCap.MES.Services;
using SonoCap.MES.Services.Interfaces;
using SonoCap.MES.Services.Model;
using System.Windows;

namespace SonoCap.MES.PreviewUI
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; }

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
            var services = new ServiceCollection();

            services.AddSingleton<GlobalModel>();
            services.AddTransient<ISerialPortWrapper, SerialPortWrapper>();
            services.AddSingleton<IMotorService, MotorService>();

            services.AddTransient<PreviewSaveViewModel>();
            services.AddTransient<PreviewSaveView>();

            return services.BuildServiceProvider();
        }
    }
}
