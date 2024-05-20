using AspectCore.DynamicProxy;
using Serilog;

namespace MES.UI.Interceptor
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class LoggingAttribute : AbstractInterceptorAttribute
    {
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
