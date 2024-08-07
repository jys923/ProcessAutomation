﻿using AspectCore.DynamicProxy;
using Serilog;
using SonoCap.Interceptors.Base;
using System.Diagnostics;

namespace SonoCap.Interceptors
{
    public class TimingInterceptor : BaseInterceptor
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
        //        Log.Information($"Method {context.ImplementationMethod.Name} executed in {stopwatch.ElapsedMilliseconds} ms");
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
                Log.Information($"Method {context.ImplementationMethod.Name} executed in {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}
