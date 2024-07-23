using AspectCore.Extensions.DependencyInjection;
using SonoCap.MES.UI.ViewModels;
using SonoCap.MES.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using SonoCap.MES.Repositories;
using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.Commons;
using System.Windows.Threading;
using AspectCore.Configuration;
using SonoCap.Interceptors;
using SonoCap.MES.UI.Services;
using SonoCap.MES.Services.Interfaces;
using SonoCap.MES.Services;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.EntityFrameworkCore.Internal;

namespace SonoCap.MES.UI
{
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;

        private DispatcherTimer _timer = default!;

        public static Dictionary<int, int> TestThresholdDict { get; private set; } = new Dictionary<int, int>();

        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
            Startup += App_Startup;
        }

        private static IServiceProvider ConfigureServices()
        {
            // Configuration 객체 생성
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // AppSettings 클래스로 설정값 매핑
            AppSettings? appSettings = new AppSettings();
            configuration.Bind(appSettings);

            LoggingConfigurator.Configure();

            IServiceCollection services = new ServiceCollection();

            //services.AddDbContext<MESDbContext>();
            // DbContext 등록 및 구성
            services.AddDbContext<MESDbContext>((serviceProvider, options) =>
            {
                ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddSerilog(dispose: true); // Serilog를 LoggerFactory에 추가
                    builder.AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information); // EF Core의 로그를 필터링하여 Serilog에게 전달
                });

                //optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
                options.UseLoggerFactory(loggerFactory); // Serilog에 EF Core 로그 리디렉션
                options.UseLazyLoadingProxies(true);
                options.EnableSensitiveDataLogging(true);

                options.UseMySql(appSettings.ConnectionStrings.MariaDBConnection, ServerVersion.AutoDetect(appSettings.ConnectionStrings.MariaDBConnection), options => options.CommandTimeout(120));
                options.UseLazyLoadingProxies(true);
            });
            services.AddScoped<MESDbContextFactory>();

            RegisterServices(services);
            RegisterRepositories(services);
            RegisterViewModels(services);
            RegisterViews(services);
            RegisterDynamicProxies(services);
            services.AddSingleton<IViewService, ViewService>();

            return services.BuildServiceProvider();
        }
        
        private void App_Startup(object sender, StartupEventArgs e)
        {
            Task.Run(() => InitializeAsync());
            SetMidnightTimer();
            SetTestThreshold();
            ShowMainView();
        }
        
        private async Task InitializeAsync()
        {
            await Services.GetRequiredService<ISharedSeqNoRepository>().InitializeAsync();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ISocketService, SocketService>();
            services.AddTransient<IExcelService, ExcelService>();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddTransient<IMotorModuleRepository, MotorModuleRepository>();
            services.AddTransient<IPcRepository, PcRepository>();
            services.AddTransient<IProbeRepository, ProbeRepository>();
            services.AddTransient<IPTRViewRepository, PTRViewRepository>();
            services.AddTransient<ISharedSeqNoRepository, SharedSeqNoRepository>();
            services.AddTransient<ITestCategoryRepository, TestCategoryRepository>();
            services.AddTransient<ITesterRepository, TesterRepository>();
            services.AddTransient<ITestRepository, TestRepository>();
            services.AddTransient<ITestTypeRepository, TestTypeRepository>();
            services.AddTransient<ITransducerRepository, TransducerRepository>();
            services.AddTransient<ITransducerModuleRepository, TransducerModuleRepository>();
            services.AddTransient<ITransducerTypeRepository, TransducerTypeRepository>();
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient(typeof(MainViewModel));
            services.AddTransient(typeof(ProbeListViewModel));
            services.AddTransient(typeof(TestListViewModel));
            services.AddTransient(typeof(TestViewModel));
        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient(s => new MainView() { DataContext = s.GetRequiredService<MainViewModel>() });
            services.AddTransient(s => new ProbeListView() { DataContext = s.GetRequiredService<ProbeListViewModel>() });
            services.AddTransient(s => new TestListView() { DataContext = s.GetRequiredService<TestListViewModel>() });
            services.AddTransient(s => new TestView() { DataContext = s.GetRequiredService<TestViewModel>() });
        }

        private static void RegisterDynamicProxies(IServiceCollection services)
        {
            services.ConfigureDynamicProxy(config =>
            {
                config.Interceptors.AddTyped<CallCountInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<ChangeHistoryInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<ExceptionLoggingInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<LoggingInterceptor>(Predicates.ForMethod("*"));
                config.Interceptors.AddTyped<ParameterLoggingInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<PerformanceInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<TimingInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<UserActivityInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<UserBehaviorInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<LoggingAttribute>(Predicates.ForService("*"));
            });
        }

        private void SetMidnightTimer()
        {
            _timer = new DispatcherTimer();
            var timeToMidnight = DateTime.Today.AddDays(1) - DateTime.Now;
            _timer.Interval = timeToMidnight;
            _timer.Tick += async (s, args) => await InitializeAsync();
            _timer.Start();
        }

        private async void SetTestThreshold()
        {
            IEnumerable<Models.TestType> testTypes = await Services.GetRequiredService<ITestTypeRepository>().GetAllAsync();

            foreach (Models.TestType testType in testTypes)
            {
                TestThresholdDict[testType.Id * 10 + 1] = testType.Threshold;
                TestThresholdDict[testType.Id * 10 + 2] = testType.Threshold;
                TestThresholdDict[testType.Id * 10 + 3] = testType.Threshold;
            }
        }

        private void ShowMainView()
        {
            //MainView? mainView = App.Current.Services.GetService<MainView>()!;
            //mainView.Show();
            IViewService viewService = Services.GetService<IViewService>()!;
            viewService.ShowMainView();
        }
    }
}
