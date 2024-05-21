using AspectCore.DynamicProxy;
using Serilog;
using SonoCap.Interceptors.Base;

namespace SonoCap.Interceptors
{
    // 인터셉터 정의
    // 이 클래스는 메서드 호출 전후에 로그를 출력하는 인터셉터 역할을 합니다.
    public class LoggingInterceptor : BaseInterceptor
    {
        protected override async Task Execute(AspectContext context, AspectDelegate next)
        {
            var methodName = context.ImplementationMethod.Name;
            var className = context.Implementation.GetType().Name;
            Log.Information($"Calling method {className}.{methodName}");
            await next(context);
            Log.Information($"Method {className}.{methodName} completed");
        }
    }
}
