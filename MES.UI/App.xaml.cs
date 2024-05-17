//#define appsettings
//#define DataTemplate

using MES.UI.Commons;
using MES.UI.Models.Context;
using MES.UI.Repositories;
using MES.UI.Repositories.interfaces;
using MES.UI.ViewModels;
using MES.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
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
#if DataTemplate
            MainView testView = App.Current.Services.GetRequiredService<MainView>();
            testView.DataContext = App.Current.Services.GetRequiredService<MainViewModel>(); // 뷰모델을 바인딩
            testView.Show();
#else
            MainView? mainView = App.Current.Services.GetService<MainView>()!;
            mainView.Show();
#endif
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

#if appsettings
            // IConfiguration을 설정합니다.
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // IConfiguration을 서비스 컨테이너에 등록합니다.
            services.AddSingleton(configuration);

            // appsettings.json 파일에서 연결 문자열을 가져옵니다.
            string MariaDBConnectionString = configuration.GetConnectionString("MariaDBConnection") ?? throw new InvalidOperationException("MariaDBConnection is null.");
#else
            string MariaDBConnectionString = MES.UI.Properties.Settings.Default.MariaDBConnection ?? throw new InvalidOperationException("MariaDBConnection is null.");
#endif
            // ILoggerFactory를 서비스에 추가
            services.AddLogging(builder =>
            {
                //builder.ClearProviders(); // 기존 로깅 프로바이더를 제거합니다.
                //builder.SetMinimumLevel(LogLevel.Trace); // 로그 레벨 설정
                builder.AddProvider(new VisualStudioOutputLoggerProvider());
                // 로그 출력을 콘솔에 추가
                //builder.AddConsole();
                //builder.AddDebug();
                //builder.AddEventLog();
                // 등등...
            });

            // DbContext를 등록합니다.
            services.AddDbContext<MESDbContext>(options =>
                options
#if DEBUG
                .EnableSensitiveDataLogging(true)
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
#endif
                .UseLazyLoadingProxies(true)
                .UseMySql(MariaDBConnectionString, ServerVersion.AutoDetect(MariaDBConnectionString)));

            // Repositories
            services.AddTransient<IMotorModuleRepository, MotorModuleRepository>();
            services.AddTransient<IPcRepository, PcRepository>();
            services.AddTransient<IProbeRepository, ProbeRepository>();
            services.AddTransient<ITestCategoryRepository, TestCategoryRepository>();
            services.AddTransient<ITesterRepository, TesterRepository>();
            services.AddTransient<ITestRepository, TestRepository>();
            services.AddTransient<ITestTypeRepository, TestTypeRepository>();
            services.AddTransient<ITransducerModuleRepository, TransducerModuleRepository>();
            services.AddTransient<ITransducerTypeRepository, TransducerTypeRepository>();

#if DataTemplate
            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<ProbeListViewModel>();
            services.AddTransient<TestListViewModel>();
            services.AddTransient<TestViewModel>();

            // Views
            services.AddTransient<MainView>();
            services.AddTransient<ProbeListView>();
            services.AddTransient<TestListView>();
            services.AddTransient<TestView>();
#else
            // ViewModels
            services.AddTransient(typeof(MainViewModel));
            services.AddTransient(typeof(ProbeListViewModel));
            services.AddTransient(typeof(TestListViewModel));
            services.AddTransient(typeof(TestViewModel));

            // Views
            services.AddTransient(s => new MainView()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });
            services.AddTransient(s => new ProbeListView()
            {
                DataContext = s.GetRequiredService<ProbeListViewModel>()
            });
            services.AddTransient(s => new TestListView()
            {
                DataContext = s.GetRequiredService<TestListViewModel>()
            });
            services.AddTransient(s => new TestView()
            {
                DataContext = s.GetRequiredService<TestViewModel>()
            });
#endif
            return services.BuildServiceProvider();
        }
    }
}