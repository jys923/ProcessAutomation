using AspectCore.DynamicProxy;
using Serilog;

namespace MES.UI.Interceptor
{
    public class UserActivityInterceptor : AbstractInterceptor
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var userId = context.Parameters.FirstOrDefault()?.ToString();
            Log.Information($"User {userId} performed an action in method {context.ImplementationMethod.Name}");
            await next(context);
        }
    }
}
