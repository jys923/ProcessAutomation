using AspectCore.DynamicProxy;
using Serilog;

namespace MES.UI.Interceptor
{
    // 인터셉터 정의
    // 이 클래스는 메서드 호출 전후에 로그를 출력하는 인터셉터 역할을 합니다.
    public class LoggingInterceptor : AbstractInterceptor
    {
        // 인터셉터의 핵심 메서드로, 메서드 호출 전후에 실행할 코드를 정의합니다.
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var className = context.Implementation.GetType().Name;
            var methodName = context.ImplementationMethod.Name;

            Log.Information($"Calling method {className}.{methodName}");
            await next(context);
            Log.Information($"Method {className}.{methodName} completed");
        }
    }
}
