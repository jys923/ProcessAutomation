using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfAppWithAop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            _serviceProvider = services.BuildDynamicProxyProvider();
            //_serviceProvider = services.BuildServiceProvider();

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            //services.AddTransient<MainWindow>();
            services.AddTransient<MainViewModel>();

            services.AddTransient(s => new MainWindow()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            services.ConfigureDynamicProxy(config =>
            {
                //config.Interceptors.AddTyped<LogInterceptor>(Predicates.ForNameSpace("WpfAppWithAop"));
                config.Interceptors.AddTyped<LogInterceptor>(Predicates.ForService("*"));
            });
        }
    }

}
