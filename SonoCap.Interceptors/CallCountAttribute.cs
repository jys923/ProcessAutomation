using AspectCore.DynamicProxy;
using Serilog;

namespace SonoCap.Interceptors
{
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
}
