using AspectCore.DynamicProxy;
using Serilog;

namespace SonoCap.MES.UI.Interceptor
{
    public class ParameterLoggingInterceptor : BaseInterceptor
    {
        //public override async Task Invoke(AspectContext context, AspectDelegate next)
        //{
        //    var parameters = context.Parameters;
        //    Log.Information($"Method {context.ImplementationMethod.Name} called with parameters: {string.Join(", ", parameters)}");
        //    await next(context);
        //    var returnValue = context.ReturnValue;
        //    Log.Information($"Method {context.ImplementationMethod.Name} returned: {returnValue}");
        //}

        protected override async Task Execute(AspectContext context, AspectDelegate next)
        {
            var parameters = context.Parameters;
            Log.Information($"Method {context.ImplementationMethod.Name} called with parameters: {string.Join(", ", parameters)}");
            await next(context);
            var returnValue = context.ReturnValue;
            Log.Information($"Method {context.ImplementationMethod.Name} returned: {returnValue}");
        }
    }
}
