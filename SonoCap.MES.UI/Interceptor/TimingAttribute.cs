using AspectCore.DynamicProxy;
using Serilog;
using System.Diagnostics;

namespace SonoCap.MES.UI.Interceptor
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class TimingAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await next(context);
            }
            finally
            {
                stopwatch.Stop();
                Log.Information($"Method {context.ImplementationMethod.Name} executed in {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}
