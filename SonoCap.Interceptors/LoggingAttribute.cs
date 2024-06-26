using AspectCore.DynamicProxy;
using Serilog;
using System.Reflection;

namespace SonoCap.Interceptors
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class LoggingAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var methodName = context.ImplementationMethod.Name;
            var className = context.Implementation.GetType().Name;
            Log.Information($"Calling method {className}.{methodName}");
            await next(context);
            Log.Information($"Method {className}.{methodName} completed");
        }
    }

    //[AttributeUsage(AttributeTargets.Method, Inherited = true)]
    //public class LoggingAttribute : AbstractInterceptorAttribute
    //{
    //    public override async Task Invoke(AspectContext context, AspectDelegate next)
    //    {
    //        var attribute = context.ServiceMethod.GetCustomAttribute<LoggingAttribute>();
    //        if (attribute != null)
    //        {
    //            var methodName = context.ImplementationMethod.Name;
    //            var className = context.Implementation.GetType().Name;
    //            Log.Information($"Calling method {className}.{methodName}");
    //            await next(context);
    //            Log.Information($"Method {className}.{methodName} completed");
    //        }
    //        else
    //        {
    //            await next(context);
    //        }
    //    }
    //}
}
