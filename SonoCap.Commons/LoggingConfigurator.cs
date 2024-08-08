using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace SonoCap.Commons
{
    public static class LoggingConfigurator
    {
        public static void Configure(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public static void Configure(LogMode mode)
        {
            switch (mode)
            {
                case LogMode.Console:
                    ConfigureConsoleLogging();
                    break;
                case LogMode.File:
                    ConfigureFileLogging();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private static void ConfigureConsoleLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .CreateLogger();
        }

        private static void ConfigureFileLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    path: "logs/log-{Date}.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    fileSizeLimitBytes: 10_000_000)
                .CreateLogger();
        }
    }

    public enum LogMode
    {
        Console,
        File
    }
}
