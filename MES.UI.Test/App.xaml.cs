using MES.UI.Test.ViewModels;
using MES.UI.Test.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MES.UI.Test
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

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            ServiceCollection? services = new ServiceCollection();

            // Stores
            //services.AddSingleton<MainNavigationStore>();

            // Services
            //services.AddSingleton<INavigationService, NavigationService>();

            // Repositories
            //services.AddTransient<IAccountRepository, AccountRepository>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<ListViewModel>();
            services.AddTransient<TestViewModel>();
            //services.AddTransient<LoginControlViewModel>();
            //services.AddTransient<SignupControlViewModel>();
            //services.AddTransient<ChangePwdControlViewModel>();
            //services.AddTransient<FindAccountControlViewModel>();
            //services.AddSingleton<MainNaviControlViewModel>();

            // Services
            //services.AddSingleton<ITestService, TestService>();

            // Views
            services.AddSingleton(s => new MainView()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            services.AddTransient(s => new ListView()
            {
                DataContext = s.GetRequiredService<ListViewModel>()
            });

            services.AddTransient(s => new TestView()
            {
                DataContext = s.GetRequiredService<TestViewModel>()
            });

            return services.BuildServiceProvider();
        }
    }

}
