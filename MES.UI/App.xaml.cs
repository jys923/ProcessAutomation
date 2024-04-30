using MES.UI.Repositories;
using MES.UI.ViewModels;
using MES.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MES.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            MainView? mainView = App.Current.Services.GetService<MainView>()!;
            mainView.Show();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            //services.AddSingleton<IFilesService, FilesService>();
            //services.AddSingleton<ISettingsService, SettingsService>();
            //services.AddSingleton<IClipboardService, ClipboardService>();
            //services.AddSingleton<IShareService, ShareService>();
            //services.AddSingleton<IEmailService, EmailService>();

            // Services
            services.AddTransient<ITesterTypeRepository, TesterTypeRepository>();
            services.AddTransient<ITestRepository, TestRepository>();
            services.AddTransient<ITestTypeRepository, TestTypeRepository>();
            services.AddTransient<IProbeSNRepository, ProbeSNRepository>();
            services.AddTransient<IProbeTypeRepository, ProbeTypeRepository>();

            // ViewModels
            services.AddTransient(typeof(MainViewModel));
            services.AddTransient(typeof(ListViewModel));
            services.AddTransient(typeof(TestViewModel));

            // Views
            services.AddSingleton(s => new MainView()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });
            services.AddSingleton(s => new ListView()
            {
                DataContext = s.GetRequiredService<ListViewModel>()
            });
            services.AddSingleton(s => new TestView()
            {
                DataContext = s.GetRequiredService<TestViewModel>()
            });

            return services.BuildServiceProvider();
        }
    }
}