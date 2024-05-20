using AspectCore.DynamicProxy;
using Serilog;

namespace MES.UI.Interceptor
{
    public class UserBehaviorInterceptor : AbstractInterceptor
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var userId = context.Parameters.FirstOrDefault()?.ToString() ?? "Unknown";
            Log.Information($"User {userId} is calling method {context.ImplementationMethod.Name}");
            await next(context);
        }
    }
}
