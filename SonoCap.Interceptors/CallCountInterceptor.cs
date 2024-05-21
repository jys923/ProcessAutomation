using AspectCore.DynamicProxy;
using Serilog;
using SonoCap.Interceptors.Base;

namespace SonoCap.Interceptors
{
    public class CallCountInterceptor : BaseInterceptor
    {
        private static int callCount = 0;
        protected override async Task Execute(AspectContext context, AspectDelegate next)
        {
            callCount++;
            Log.Information($"Method {context.ImplementationMethod.Name} called {callCount} times");
            await next(context);
        }
    }
}
