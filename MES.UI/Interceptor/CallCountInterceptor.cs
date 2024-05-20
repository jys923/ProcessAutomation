using AspectCore.DynamicProxy;
using Serilog;

namespace MES.UI.Interceptor
{
    public class CallCountInterceptor : BaseInterceptor
    {
        private static int callCount = 0;
        protected override async Task Execute(AspectContext context, AspectDelegate next)
        {
            callCount++;
            Log.Information($"Method {context.ImplementationMethod.Name} called {callCount} times");
            await next(context);
        }
    }

    public abstract class BaseInterceptor : AbstractInterceptor
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var methodName = context.ImplementationMethod.Name;
            if (!methodName.Contains("OnProperty"))
            {
                await Execute(context, next);
            }
            else
            {
                await next(context);
            }
        }

        protected abstract Task Execute(AspectContext context, AspectDelegate next);
    }
}
