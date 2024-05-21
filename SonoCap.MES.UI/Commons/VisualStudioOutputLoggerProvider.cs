using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SonoCap.MES.UI.Commons
{
    //// ILoggerFactory를 서비스에 추가
    //services.AddLogging(builder =>
    //{
    //    //builder.ClearProviders(); // 기존 로깅 프로바이더를 제거합니다.
    //    //builder.SetMinimumLevel(LogLevel.Trace); // 로그 레벨 설정
    //    builder.AddProvider(new VisualStudioOutputLoggerProvider());
    //    // 로그 출력을 콘솔에 추가
    //    //builder.AddConsole();
    //    //builder.AddDebug();
    //    //builder.AddEventLog();
    //    // 등등...
    //});
    public class VisualStudioOutputLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new VisualStudioOutputLogger(categoryName);
        }

        public void Dispose()
        {
        }
    }

    public class VisualStudioOutputLogger : ILogger
    {
        private readonly string _categoryName;

        public VisualStudioOutputLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Trace.WriteLine(formatter(state, exception), _categoryName);
        }
    }
}
