//#define appsettings
//#define DataTemplate

using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Azure.Identity;
using MES.UI.Context;
using MES.UI.Interceptor;
using MES.UI.Repositories;
using MES.UI.Repositories.interfaces;
using MES.UI.ViewModels;
using MES.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
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
            //Debug: 비주얼스튜디오 출력창
            //Log.Logger = new LoggerConfiguration()
            //    .WriteTo.Debug(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {ClassName}.{MethodName} {Message:lj}{NewLine}{Exception}")
            //    .CreateLogger();

            //Console: PowerShell 따로
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            IServiceCollection services = new ServiceCollection();

            // AspectCore의 동적 프록시 설정을 구성합니다.
            services.ConfigureDynamicProxy(config =>
            {
                // 모든 서비스에 LoggingInterceptor를 적용하도록 설정합니다.
                config.Interceptors.AddTyped<LoggingInterceptor>(Predicates.ForService("*"));
            });

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
            string MariaDBConnectionString = MES.UI.Properties.Settings.Default.MariaDBConnection
                                             ?? throw new InvalidOperationException("MariaDBConnection is null.");
#endif
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(dispose: true); // Serilog를 LoggerFactory에 추가
                builder.AddFilter((category, level) =>
                    category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information); // EF Core의 로그를 필터링하여 Serilog에게 전달
            });
            // DbContext를 등록합니다.
            services.AddDbContext<MESDbContext>(options =>
                options
#if DEBUG
                .EnableSensitiveDataLogging(true)
                //.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
#endif
                .UseLoggerFactory(loggerFactory) // Serilog에 EF Core 로그 리디렉션
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

            //// AspectCore의 동적 프록시 설정을 구성합니다.
            //services.ConfigureDynamicProxy(config =>
            //{
            //    // 모든 서비스에 LoggingInterceptor를 적용하도록 설정합니다.
            //    config.Interceptors.AddTyped<LoggingInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<ExceptionLoggingInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<ParameterLoggingInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<TimingInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<PerformanceInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<CallCountInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<UserBehaviorInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<ChangeHistoryInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<UserActivityInterceptor>(Predicates.ForService("*"));
            //});

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
            // 응용 프로그램의 서비스 공급자로 설정합니다.
            //return services.BuildServiceProvider();
            return services.BuildDynamicProxyProvider();
        }
    }
}