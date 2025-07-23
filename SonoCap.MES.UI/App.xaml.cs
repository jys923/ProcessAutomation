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
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;
using SonoCap.MES.UI.Services;
using SonoCap.MES.UI.Services.Interfaces;
using SonoCap.WpfCommons;
using System.Reflection;
using SonoCap.Commons.Logging;

namespace SonoCap.MES.UI
{
    public partial class App : Application
    {
        public static string appTempDir = Path.Combine(Path.GetTempPath(), Assembly.GetEntryAssembly()?.GetName().Name ?? "SonoCap.MES.UI");

        public new static App Current => (App)Application.Current;

        private DispatcherTimer _timer = default!;

        public static Dictionary<int, int> TestThresholdDict { get; private set; } = new Dictionary<int, int>();

        public IServiceProvider Services { get; }

        private readonly SonoCap.MES.Services.Interfaces.IMotorService _motorService;

        public static AppSettings appSettings { get; set; } = new AppSettings();

        private Stopwatch sw = default!;

        public App()
        {
            sw = Stopwatch.StartNew();
            Services = ConfigureServices();
            Log.Information($"SonoCap.MES.UI 시작 : {sw.ElapsedMilliseconds}ms");
            _motorService = Services.GetRequiredService<SonoCap.MES.Services.Interfaces.IMotorService>(); // 싱글톤 유지
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e); // 기본 OnStartup 메서드를 호출하여 기본 초기화 수행

            Utilities.ResetFolder(appTempDir);
            // 비동기 초기화 작업을 시작합니다.
            //await Task.Run(() => InitializeAsync());
            //await Task.Run(() => SetTestThreshold());
            try
            {
                var context = Services.GetRequiredService<MESDbContext>();
                if (App.appSettings.DbSettings.AutoMigrate)
                {
                    context.Database.Migrate(); // 테이블이 없으면 생성 + 마이그레이션 자동 적용
                    Log.Information("DB 마이그레이션 자동 적용 완료");
                    await context.SeedAsync(); // 
                    Log.Information("DB SeedAsync");
                }
                await context.Database.OpenConnectionAsync();
            }
            catch (Exception ex)
            {
                Log.Information("DB 연결 실패: " + ex.Message);
                MessageBox.Show("DB 연결에 실패했습니다.\n\n" + ex.Message, "DB 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
            
            // 비동기 초기화 작업을 시작합니다.
            await Task.WhenAll(
                InitializeAsync(),
                SetTestThreshold()
            );

            Log.Information($"DB 초기화: {sw.ElapsedMilliseconds}ms");

            // 비동기 작업이 완료된 후에 나머지 초기화 작업을 수행합니다.
            SetMidnightTimer();
            SetPath();
            Log.Information($"View 표시까지: {sw.ElapsedMilliseconds}ms");
            //ShowMainView();
            ShowFirstView();

            Log.Information($"전체 초기화: {sw.ElapsedMilliseconds}ms");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (_motorService is IDisposable disposableMotor)
            {
                Log.Information("Application exiting: Disposing motor service.");
                disposableMotor.Dispose();
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            IConfiguration configuration = ConfigureAppSettings();
            configuration.Bind(appSettings);

            LoggingConfigurator.Configure(appSettings.Serilog); // ← 이렇게 바뀜
            Logger.Initialize(Log.Logger);

            IServiceCollection services = new ServiceCollection();

            ConfigureDbContext(services, appSettings);

            //services.AddScoped<MESDbContextFactory>();
            RegisterModels(services);
            RegisterServices(services);
            RegisterRepositories(services);
            RegisterViewModels(services);
            RegisterViews(services);
            //RegisterDynamicProxies(services);

            return services.BuildServiceProvider();
        }

        private static IConfiguration ConfigureAppSettings()
        {
            var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"; // 기본값은 Production

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true) // 환경별 파일 로드
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
                        category == DbLoggerCategory.Database.Command.Name && level >= LogLevel.Information);
                });

                options.UseLoggerFactory(loggerFactory);
                options.UseLazyLoadingProxies(true);
                options.EnableSensitiveDataLogging(false);

                options.UseMySql(appSettings.ConnectionStrings.MariaDBConnection, ServerVersion.AutoDetect(appSettings.ConnectionStrings.MariaDBConnection), options => options.CommandTimeout(30));
            }, ServiceLifetime.Transient);
        }

        private async Task InitializeAsync()
        {
            await Services.GetRequiredService<ISharedSeqNoRepository>().InitializeAsync();
        }

        public static void RegisterModels(IServiceCollection services)
        {
            services.AddSingleton<MES.Services.Model.GlobalModel>(); // 다른 모델이 있다면 여기에 추가
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ProbeService>();
            services.AddTransient<TestingManagementService>();
            services.AddTransient<SonoCap.MES.Services.ISerialPortWrapper, SonoCap.MES.Services.SerialPortWrapper>();
            services.AddTransient<SonoCap.MES.Services.Interfaces.IExcelService, SonoCap.MES.Services.ExcelService>();
            services.AddTransient<SonoCap.MES.Services.Interfaces.IMotorService, SonoCap.MES.Services.MotorService>();
            services.AddSingleton<IViewService, ViewService>();
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
            services.AddTransient(typeof(InputBoxProbeViewModel));
            services.AddTransient(typeof(InputBoxMotorViewModel));
            services.AddTransient(typeof(MainViewModel));
            services.AddTransient(typeof(FirstViewModel));
            services.AddTransient(typeof(ProbeListViewModel));
            services.AddTransient(typeof(TestListViewModel));
            services.AddTransient(typeof(TestingViewModel));
            services.AddTransient(typeof(TestViewModel));
            services.AddTransient(typeof(ProbeViewModel));
            services.AddTransient(typeof(AboutViewModel));
        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient(typeof(InputBoxProbeView));
            services.AddTransient(typeof(InputBoxMotorView));
            services.AddTransient(typeof(MainView));
            services.AddTransient(typeof(FirstView));
            services.AddTransient(typeof(ProbeListView));
            services.AddTransient(typeof(TestListView));
            services.AddTransient(typeof(TestingView));
            services.AddTransient(typeof(TestView));
            services.AddTransient(typeof(ProbeView));
            services.AddTransient(typeof(AboutView));
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
