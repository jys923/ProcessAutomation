using AspectCore.DynamicProxy;
using Serilog;

namespace SonoCap.MES.UI.Interceptor
{
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
}
