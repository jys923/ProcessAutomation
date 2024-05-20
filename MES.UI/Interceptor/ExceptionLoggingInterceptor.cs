using AspectCore.DynamicProxy;
using Serilog;

namespace MES.UI.Interceptor
{
    public class ExceptionLoggingInterceptor : AbstractInterceptor
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Log.Error($"Exception in method {context.ImplementationMethod.Name}: {ex.Message}");
                throw;
            }
        }
    }
}
