using AspectCore.DynamicProxy;
using Serilog;

namespace MES.UI.Interceptor
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ExceptionLoggingAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Exception in method {context.ImplementationMethod.Name}");
                throw;
            }
        }
    }
}
