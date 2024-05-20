using AspectCore.DynamicProxy;
using Serilog;

namespace MES.UI.Interceptor
{
    public class ExceptionLoggingInterceptor : BaseInterceptor
    {
        //public override async Task Invoke(AspectContext context, AspectDelegate next)
        //{
        //    try
        //    {
        //        await next(context);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error($"Exception in method {context.ImplementationMethod.Name}: {ex.Message}");
        //        throw;
        //    }
        //}

        protected override async Task Execute(AspectContext context, AspectDelegate next)
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
