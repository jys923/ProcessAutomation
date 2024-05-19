using AspectCore.Configuration; // AspectCore의 구성 관련 네임스페이스
using AspectCore.DynamicProxy; // AspectCore의 동적 프록시 관련 네임스페이스
using AspectCore.Extensions.DependencyInjection; // AspectCore의 DI 확장 관련 네임스페이스
using Microsoft.Extensions.DependencyInjection; // Microsoft의 DI 관련 네임스페이스

namespace AspectCoreSample
{
    // Aspect(관점)를 정의하는 클래스입니다.
    // 이 클래스는 메서드 호출 전후에 로그를 출력하는 인터셉터 역할을 합니다.
    public class LoggingInterceptor : AbstractInterceptor
    {
        // 인터셉터의 핵심 메서드로, 메서드 호출 전후에 실행할 코드를 정의합니다.
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            Console.WriteLine($"Calling method {context.ImplementationMethod.Name}"); // 메서드 호출 전 로그 출력
            await next(context); // 실제 메서드 호출
            Console.WriteLine($"Method {context.ImplementationMethod.Name} completed"); // 메서드 호출 후 로그 출력
        }
    }

    // Aspect를 적용할 서비스 인터페이스를 정의합니다.
    public interface IMyService
    {
        void DoWork(); // 예제 메서드 정의
    }

    // 서비스 인터페이스를 구현하는 클래스입니다.
    public class MyService : IMyService
    {
        public void DoWork()
        {
            Console.WriteLine("Doing work..."); // 실제 작업 수행 로직
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // 서비스 컬렉션을 생성합니다.
            ServiceCollection services = new ServiceCollection();

            // IMyService를 구현하는 MyService 클래스를 DI 컨테이너에 등록합니다.
            services.AddTransient<IMyService, MyService>();

            // AspectCore의 동적 프록시 설정을 구성합니다.
            services.ConfigureDynamicProxy(config =>
            {
                // 모든 서비스에 LoggingInterceptor를 적용하도록 설정합니다.
                config.Interceptors.AddTyped<LoggingInterceptor>(Predicates.ForService("*"));
            });

            // AspectCore DI로 서비스 제공자를 빌드합니다.
            ServiceProvider serviceProvider = services.BuildDynamicProxyProvider();

            // 서비스 제공자로부터 IMyService 인스턴스를 가져옵니다.
            IMyService? myService = serviceProvider.GetService<IMyService>();

            // IMyService의 메서드를 호출합니다.
            // 이 호출은 LoggingInterceptor에 의해 인터셉트됩니다.
            myService.DoWork();
        }
    }
}
