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
using VILib;
using SonoCap.MES.Services.Interfaces;
using SonoCap.MES.Services;

namespace SonoCap.MES.UI
{
    public partial class App : Application
    {
        private DispatcherTimer _timer = default!;

        public new static App Current => (App)Application.Current;

        public static Dictionary<int, int> TestThresholdDict { get; private set; } = new Dictionary<int, int>();

        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
            Startup += App_Startup;
        }

        private static IServiceProvider ConfigureServices()
        {
            LoggingConfigurator.Configure();

            IServiceCollection services = new ServiceCollection();

            services.AddDbContext<MESDbContext>();
            services.AddScoped<MESDbContextFactory>();

            RegisterRepositories(services);
            RegisterViewModels(services);
            RegisterViews(services);
            RegisterDynamicProxies(services);
            services.AddSingleton<IViewService, ViewService>();
            services.AddSingleton<VILibWrapper>();

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
            //services.AddSingleton<ISocketService, SocketService>();
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
