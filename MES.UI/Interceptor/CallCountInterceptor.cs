using AspectCore.DynamicProxy;
using Serilog;

namespace MES.UI.Interceptor
{
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
}
