using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace SonoCap.Commons
{
    public static class LoggingConfigurator
    {
        public static void Configure()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .CreateLogger();
        }
    }
}
