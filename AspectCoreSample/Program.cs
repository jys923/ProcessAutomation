using AspectCore.Configuration; // AspectCore의 구성 관련 네임스페이스
using AspectCore.DynamicProxy; // AspectCore의 동적 프록시 관련 네임스페이스
using AspectCore.Extensions.DependencyInjection; // AspectCore의 DI 확장 관련 네임스페이스
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics; // Microsoft의 DI 관련 네임스페이스

namespace AspectCoreSample
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class LoggingAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            Log.Information($"Calling method {context.ImplementationMethod.Name}");
            await next(context);
            Log.Information($"Method {context.ImplementationMethod.Name} completed");
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ExceptionLoggingAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Exception in method {context.ImplementationMethod.Name}");
                throw;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ParameterLoggingAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var parameters = context.Parameters;
            Log.Information($"Method {context.ImplementationMethod.Name} called with parameters: {string.Join(", ", parameters)}");
            await next(context);
            var returnValue = context.ReturnValue;
            Log.Information($"Method {context.ImplementationMethod.Name} returned: {returnValue}");
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class TimingAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await next(context);
            }
            finally
            {
                stopwatch.Stop();
                Log.Information($"Method {context.ImplementationMethod.Name} executed in {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class PerformanceAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await next(context);
            }
            finally
            {
                stopwatch.Stop();
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                if (elapsedMilliseconds > 1000) // 1초 이상 걸리면 로그 출력
                {
                    Log.Warning($"Performance issue: Method {context.ImplementationMethod.Name} took {elapsedMilliseconds} ms");
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CallCountAttribute : AbstractInterceptorAttribute
    {
        private static int callCount = 0;

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            callCount++;
            Log.Information($"Method {context.ImplementationMethod.Name} called {callCount} times");
            await next(context);
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class UserBehaviorAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var userId = context.Parameters.FirstOrDefault()?.ToString();
            Log.Information($"User {userId} is calling method {context.ImplementationMethod.Name}");
            await next(context);
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ChangeHistoryAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            // 매개변수가 있는지 확인합니다.
            if (context.Parameters.Count() > 0)
            {
                try
                {
                    var originalValue = context.Parameters.ElementAtOrDefault(0); // 첫 번째 매개변수의 원래 값
                    await next(context);
                    var newValue = context.Parameters[0]; // 첫 번째 매개변수의 새로운 값
                    Log.Information($"Change history: {originalValue} -> {newValue}");
                }
                catch (IndexOutOfRangeException ex)
                {
                    Log.Error(ex, $"Error occurred in ChangeHistoryInterceptor: {ex.Message}");
                    throw;
                }
            }
            else
            {
                // 파라미터가 없는 경우 로그에 기록합니다.
                Log.Warning("ChangeHistoryInterceptor: Method called with no parameters.");
                await next(context);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class UserActivityAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var userId = context.Parameters.FirstOrDefault()?.ToString();
            Log.Information($"User {userId} performed an action in method {context.ImplementationMethod.Name}");
            await next(context);
        }
    }
    // 인터셉터 정의
    // 이 클래스는 메서드 호출 전후에 로그를 출력하는 인터셉터 역할을 합니다.
    public class LoggingInterceptor : AbstractInterceptor
    {
        // 인터셉터의 핵심 메서드로, 메서드 호출 전후에 실행할 코드를 정의합니다.
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            Log.Information($"Calling method {context.ImplementationMethod.Name}"); // 메서드 호출 전 로그 출력
            await next(context); // 실제 메서드 호출
            Log.Information($"Method {context.ImplementationMethod.Name} completed"); // 메서드 호출 후 로그 출력
        }
    }

    public class ExceptionLoggingInterceptor : AbstractInterceptor
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Log.Error($"Exception in method {context.ImplementationMethod.Name}: {ex.Message}");
                throw;
            }
        }
    }

    public class ParameterLoggingInterceptor : AbstractInterceptor
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var parameters = context.Parameters;
            Log.Information($"Method {context.ImplementationMethod.Name} called with parameters: {string.Join(", ", parameters)}");
            await next(context);
            var returnValue = context.ReturnValue;
            Log.Information($"Method {context.ImplementationMethod.Name} returned: {returnValue}");
        }
    }

    public class TimingInterceptor : AbstractInterceptor
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await next(context);
            }
            finally
            {
                stopwatch.Stop();
                Log.Information($"Method {context.ImplementationMethod.Name} executed in {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }

    public class PerformanceInterceptor : AbstractInterceptor
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await next(context);
            }
            finally
            {
                stopwatch.Stop();
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                if (elapsedMilliseconds > 1000) // 1초 이상 걸리면 로그 출력
                {
                    Log.Warning($"Performance issue: Method {context.ImplementationMethod.Name} took {elapsedMilliseconds} ms");
                }
            }
        }
    }

    public class CallCountInterceptor : AbstractInterceptor
    {
        private static int callCount = 0;

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            callCount++;
            Log.Information($"Method {context.ImplementationMethod.Name} called {callCount} times");
            await next(context);
        }
    }

    public class UserBehaviorInterceptor : AbstractInterceptor
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var userId = context.Parameters.FirstOrDefault()?.ToString() ?? "Unknown";
            Log.Information($"User {userId} is calling method {context.ImplementationMethod.Name}");
            await next(context);
        }
    }

    public class ChangeHistoryInterceptor : AbstractInterceptor
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            // 매개변수가 있는지 확인합니다.
            if (context.Parameters.Count() > 0)
            {
                try
                {
                    var originalValue = context.Parameters.ElementAtOrDefault(0); // 첫 번째 매개변수의 원래 값
                    await next(context);
                    var newValue = context.Parameters[0]; // 첫 번째 매개변수의 새로운 값
                    Log.Information($"Change history: {originalValue} -> {newValue}");
                }
                catch (IndexOutOfRangeException ex)
                {
                    Log.Error(ex, $"Error occurred in ChangeHistoryInterceptor: {ex.Message}");
                    throw;
                }
            }
            else
            {
                // 파라미터가 없는 경우 로그에 기록합니다.
                Log.Warning("ChangeHistoryInterceptor: Method called with no parameters.");
                await next(context);
            }
        }
    }


    public class UserActivityInterceptor : AbstractInterceptor
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var userId = context.Parameters.FirstOrDefault()?.ToString();
            Log.Information($"User {userId} performed an action in method {context.ImplementationMethod.Name}");
            await next(context);
        }
    }

    // 서비스 인터페이스 및 구현
    public interface IMyService
    {
        void DoWork();
        void DoWorkWithParameters(string userId);
        void DoWorkErr();
        string GetData();
    }

    public class MyService : IMyService
    {
        public void DoWork()
        {
            Console.WriteLine("Doing work...");
            Thread.Sleep(500);
        }

        public void DoWorkErr()
        {
            Console.WriteLine("Doing work err...");
            throw new InvalidOperationException("This is a test exception.");
        }

        public void DoWorkWithParameters(string userId)
        {
            Console.WriteLine($"Doing work with user {userId}...");
        }

        public string GetData()
        {
            return "Data from service";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            //.WriteTo.Console()
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

            // 서비스 컬렉션을 생성합니다.
            ServiceCollection services = new ServiceCollection();

            // IMyService를 구현하는 MyService 클래스를 DI 컨테이너에 등록합니다.
            services.AddTransient<IMyService, MyService>();

            // AspectCore의 동적 프록시 설정을 구성합니다.
            services.ConfigureDynamicProxy(config =>
            {
                // 모든 서비스에 LoggingInterceptor를 적용하도록 설정합니다.
                config.Interceptors.AddTyped<LoggingInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<ExceptionLoggingInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<ParameterLoggingInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<TimingInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<PerformanceInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<CallCountInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<UserBehaviorInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<ChangeHistoryInterceptor>(Predicates.ForService("*"));
                config.Interceptors.AddTyped<UserActivityInterceptor>(Predicates.ForService("*"));
            });

            // AspectCore DI로 서비스 제공자를 빌드합니다.
            //ServiceProvider serviceProvider = services.BuildProvider(); 
            //BuildProvider 랑 다름 구분해서 사용
            ServiceProvider serviceProvider = services.BuildDynamicProxyProvider();

            // 서비스 제공자로부터 IMyService 인스턴스를 가져옵니다.
            IMyService? myService = serviceProvider.GetService<IMyService>();

            // DoWork 호출 (인터셉터 적용 됨)
            myService.DoWork();

            try
            {
                // DoWorkErr 호출 (인터셉터 적용 됨)
                myService.DoWorkErr();
            }
            catch (Exception ex)
            {
                Log.Error($"An exception occurred: {ex.Message}");
            }

            // DoWorkWithParameters 호출 (인터셉터 적용 됨)
            myService.DoWorkWithParameters("User123");

            // GetData 호출 (인터셉터 적용 됨)
            var data = myService.GetData();
            Console.WriteLine($"Returned data: {data}");
        }
    }
}
