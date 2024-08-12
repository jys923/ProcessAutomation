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
using SonoCap.MES.UI.Commons;

namespace SonoCap.MES.UI
{
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;

        private DispatcherTimer _timer = default!;

        public static Dictionary<int, int> TestThresholdDict { get; private set; } = new Dictionary<int, int>();

        public IServiceProvider Services { get; }

        public static AppSettings appSettings { get; set; } = new AppSettings();

        public App()
        {
            Services = ConfigureServices();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e); // 기본 OnStartup 메서드를 호출하여 기본 초기화 수행

            // 비동기 초기화 작업을 시작합니다.
            //Task.Run(() => InitializeAsync());
            //Task.Run(() => SetTestThreshold());
            // 비동기 초기화 작업을 시작합니다.
            await Task.WhenAll(
                InitializeAsync(),
                SetTestThreshold()
            );

            // 비동기 작업이 완료된 후에 나머지 초기화 작업을 수행합니다.
            SetMidnightTimer();
            SetPath();
            ShowFirstView();
        }

        private static IServiceProvider ConfigureServices()
        {
            IConfiguration configuration = ConfigureAppSettings();
            configuration.Bind(appSettings);

            LoggingConfigurator.Configure(configuration);

            IServiceCollection services = new ServiceCollection();

            ConfigureDbContext(services, appSettings);
            
            services.AddScoped<MESDbContextFactory>();

            RegisterServices(services);
            RegisterRepositories(services);
            RegisterViewModels(services);
            RegisterViews(services);
            RegisterDynamicProxies(services);
            services.AddSingleton<IViewService, ViewService>();

            return services.BuildServiceProvider();
        }

        private static IConfiguration ConfigureAppSettings()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }

        private static void ConfigureDbContext(IServiceCollection services, AppSettings appSettings)
        {
            services.AddDbContext<MESDbContext>((serviceProvider, options) =>
            {
                ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddSerilog(dispose: true);
                    builder.AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information);
                });

                options.UseLoggerFactory(loggerFactory);
                options.UseLazyLoadingProxies(true);
                options.EnableSensitiveDataLogging();

                options.UseMySql(appSettings.ConnectionStrings.MariaDBConnection, ServerVersion.AutoDetect(appSettings.ConnectionStrings.MariaDBConnection), options => options.CommandTimeout(120));
            });
        }

        private async Task InitializeAsync()
        {
            await Services.GetRequiredService<ISharedSeqNoRepository>().InitializeAsync();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ISocketService, SocketService>();
            services.AddTransient<IExcelService, ExcelService>();
            services.AddTransient<MotorService>();
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
            services.AddTransient(typeof(FirstViewModel));
            services.AddTransient(typeof(ProbeListViewModel));
            services.AddTransient(typeof(TestListViewModel));
            services.AddTransient(typeof(TestingViewModel));
            services.AddTransient(typeof(TestViewModel));
            services.AddTransient(typeof(ProbeViewModel));
        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient(typeof(MainView));
            services.AddTransient(typeof(FirstView));
            services.AddTransient(typeof(ProbeListView));
            services.AddTransient(typeof(TestListView));
            services.AddTransient(typeof(TestingView));
            services.AddTransient(typeof(TestView));
            services.AddTransient(typeof(ProbeView));
            //services.AddTransient(s => new MainView() { DataContext = s.GetRequiredService<MainViewModel>() });
            //services.AddTransient(s => new FirstView() { DataContext = s.GetRequiredService<FirstViewModel>() });
            //services.AddTransient(s => new ProbeListView() { DataContext = s.GetRequiredService<ProbeListViewModel>() });
            //services.AddTransient(s => new TestListView() { DataContext = s.GetRequiredService<TestListViewModel>() });
            //services.AddTransient(s => new TestingView() { DataContext = s.GetRequiredService<TestingViewModel>() });
            //services.AddTransient(s => new TestView() { DataContext = s.GetRequiredService<TestViewModel>() });
            //services.AddTransient(s => new ProbeView() { DataContext = s.GetRequiredService<ProbeViewModel>() });
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

        private async Task SetTestThreshold()
        {
            IEnumerable<Models.TestType> testTypes = await Services.GetRequiredService<ITestTypeRepository>().GetAllAsync();

            foreach (Models.TestType testType in testTypes)
            {
                TestThresholdDict[testType.Id * 10 + 1] = testType.Threshold;
                TestThresholdDict[testType.Id * 10 + 2] = testType.Threshold;
                TestThresholdDict[testType.Id * 10 + 3] = testType.Threshold;
            }
        }

        private void SetPath()
        {
            Utilities.EnsureFolderExists(appSettings.Path.ImportExcel);
            Utilities.EnsureFolderExists(appSettings.Path.ExportExcel);
            Utilities.EnsureFolderExists(appSettings.Path.ExportImg);
        }

        private void ShowMainView()
        {
            //MainView? mainView = App.Current.Services.GetService<MainView>()!;
            //mainView.Show();
            IViewService viewService = Services.GetService<IViewService>()!;
            viewService.ShowMainView();
        }

        private void ShowFirstView()
        {
            IViewService viewService = Services.GetService<IViewService>()!;
            viewService.ShowFirstView();
        }
    }
}
