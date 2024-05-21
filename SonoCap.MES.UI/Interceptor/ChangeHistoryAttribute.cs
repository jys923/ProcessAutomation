using AspectCore.DynamicProxy;
using Serilog;

namespace SonoCap.MES.UI.Interceptor
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ChangeHistoryAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            // 매개변수가 있는지 확인합니다.
            if (context.Parameters.Count() > 0)
            {
                try
                {
                    var originalValue = context.Parameters.ElementAtOrDefault(0); // 첫 번째 매개변수의 원래 값
                    await next(context);
                    var newValue = context.Parameters[0]; // 첫 번째 매개변수의 새로운 값
                    Log.Information($"Change history: {originalValue} -> {newValue}");
                }
                catch (IndexOutOfRangeException ex)
                {
                    Log.Error(ex, $"Error occurred in ChangeHistoryInterceptor: {ex.Message}");
                    throw;
                }
            }
            else
            {
                // 파라미터가 없는 경우 로그에 기록합니다.
                Log.Warning("ChangeHistoryInterceptor: Method called with no parameters.");
                await next(context);
            }
        }
    }
}
