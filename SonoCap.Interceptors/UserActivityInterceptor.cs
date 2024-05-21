using AspectCore.DynamicProxy;
using Serilog;
using SonoCap.Interceptors.Base;

namespace SonoCap.Interceptors
{
    public class UserActivityInterceptor : BaseInterceptor
    {
        //public override async Task Invoke(AspectContext context, AspectDelegate next)
        //{
        //    var userId = context.Parameters.FirstOrDefault()?.ToString();
        //    Log.Information($"User {userId} performed an action in method {context.ImplementationMethod.Name}");
        //    await next(context);
        //}

        protected override async Task Execute(AspectContext context, AspectDelegate next)
        {
            var userId = context.Parameters.FirstOrDefault()?.ToString();
            Log.Information($"User {userId} performed an action in method {context.ImplementationMethod.Name}");
            await next(context);
        }
    }
}
