using AspectCore.DynamicProxy;
using Serilog;
using System.Diagnostics;

namespace MES.UI.Interceptor
{
    public class PerformanceInterceptor : BaseInterceptor
    {
        //public override async Task Invoke(AspectContext context, AspectDelegate next)
        //{
        //    var stopwatch = Stopwatch.StartNew();
        //    try
        //    {
        //        await next(context);
        //    }
        //    finally
        //    {
        //        stopwatch.Stop();
        //        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        //        if (elapsedMilliseconds > 1000) // 1초 이상 걸리면 로그 출력
        //        {
        //            Log.Warning($"Performance issue: Method {context.ImplementationMethod.Name} took {elapsedMilliseconds} ms");
        //        }
        //    }
        //}

        protected override async Task Execute(AspectContext context, AspectDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await next(context);
            }
            finally
            {
                stopwatch.Stop();
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                if (elapsedMilliseconds > 1000) // 1초 이상 걸리면 로그 출력
                {
                    Log.Warning($"Performance issue: Method {context.ImplementationMethod.Name} took {elapsedMilliseconds} ms");
                }
            }
        }
    }
}
