using AspectCore.DynamicProxy;

namespace WpfAppWithAop
{
    public class LogInterceptor : AbstractInterceptor
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            Console.WriteLine($"Calling method: {context.ImplementationMethod.Name}");
            await next(context);
            Console.WriteLine($"Method {context.ImplementationMethod.Name} executed");
        }
    }
}