//#define appsettings
//#define DataTemplate

using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using SonoCap.Interceptors;
using SonoCap.MES.UI.ViewModels;
using SonoCap.MES.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Windows;
using SonoCap.MES.Repositories;
using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.Validation;

namespace SonoCap.MES.UI
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
            SetTestThreshold();
            MainView? mainView = App.Current.Services.GetService<MainView>()!;
            mainView.Show();
#endif
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        public static int Test11 { get; private set; }
        public static int Test12 { get; private set; }
        public static int Test13 { get; private set; }

        public static int Test21 { get; private set; }
        public static int Test22 { get; private set; }
        public static int Test23 { get; private set; }

        public static int Test31 { get; private set; }
        public static int Test32 { get; private set; }
        public static int Test33 { get; private set; }

        public static Dictionary<int, int> _testThresholdDict { get; private set; } = new Dictionary<int, int>();

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

            services.AddDbContext<MESDbContext>();

            // Repositories
            services.AddTransient<IMotorModuleRepository, MotorModuleRepository>();
            services.AddTransient<IPcRepository, PcRepository>();
            services.AddTransient<IProbeRepository, ProbeRepository>();
            services.AddTransient<IPTRViewRepository, PTRViewRepository>();
            services.AddTransient<ITestCategoryRepository, TestCategoryRepository>();
            services.AddTransient<ITesterRepository, TesterRepository>();
            services.AddTransient<ITestRepository, TestRepository>();
            services.AddTransient<ITestTypeRepository, TestTypeRepository>();
            services.AddTransient<ITransducerRepository, TransducerRepository>();
            services.AddTransient<ITransducerModuleRepository, TransducerModuleRepository>();
            services.AddTransient<ITransducerTypeRepository, TransducerTypeRepository>();

            // AspectCore의 동적 프록시 설정을 구성합니다.
            //services.ConfigureDynamicProxy(config =>
            //{
            //    // 모든 서비스에 LoggingInterceptor를 적용하도록 설정합니다.
            //    config.Interceptors.AddTyped<CallCountInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<ChangeHistoryInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<ExceptionLoggingInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<LoggingInterceptor>(Predicates.ForMethod("*"));
            //    config.Interceptors.AddTyped<ParameterLoggingInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<PerformanceInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<TimingInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<UserActivityInterceptor>(Predicates.ForService("*"));
            //    config.Interceptors.AddTyped<UserBehaviorInterceptor>(Predicates.ForService("*"));
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

        private async void SetTestThreshold()
        {
            // TestTypeRepository를 서비스로부터 가져옵니다.
            ITestTypeRepository testTypeRepository = App.Current.Services.GetRequiredService<ITestTypeRepository>();

            // TestType 데이터를 가져오기 위한 메서드를 호출합니다. (예: GetAllTestTypesAsync())
            IEnumerable<Models.TestType> testTypes = await testTypeRepository.GetAllAsync();

            // 가져온 데이터를 처리합니다.
            foreach (Models.TestType testType in testTypes)
            {
                switch (testType.Id)
                {
                    case 1:
                        _testThresholdDict[11] = testType.Threshold;
                        _testThresholdDict[21] = testType.Threshold;
                        _testThresholdDict[31] = testType.Threshold;
                        break;
                    case 2:
                        _testThresholdDict[12] = testType.Threshold;
                        _testThresholdDict[22] = testType.Threshold;
                        _testThresholdDict[32] = testType.Threshold;
                        break;
                    case 3:
                        _testThresholdDict[13] = testType.Threshold;
                        _testThresholdDict[23] = testType.Threshold;
                        _testThresholdDict[33] = testType.Threshold;
                        break;
                }
            }
        }
    }
}