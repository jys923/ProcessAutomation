using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MES.UI.Commons
{
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
