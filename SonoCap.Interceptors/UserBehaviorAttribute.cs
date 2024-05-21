using AspectCore.DynamicProxy;
using Serilog;

namespace SonoCap.Interceptors
{
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
}
