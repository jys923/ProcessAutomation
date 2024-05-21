using AspectCore.DynamicProxy;

namespace SonoCap.Interceptors.Base
{
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
