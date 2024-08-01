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

        public static void Configure(int mode)
        {
            switch (mode)
            {
                case 0:
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .WriteTo.Console(
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                            //outputTemplate: "{Timestamp:HH:mm:ss,fff} {Level:u3} {FileName} [{MemberName}] {Message:lj}{NewLine}{Exception}",
                            //outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u4}] ({SourceContext:l}.{MemberName}) {Message:lj}{NewLine}{Exception}",
                            //outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message} (at {ClassName} class in {MethodName} method){NewLine}{Exception}",
                            theme: AnsiConsoleTheme.Code)
                        .CreateLogger();
                    break;
                case 1:
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .WriteTo.File(
                            path: "logs/log-{Date}.txt",
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                            rollingInterval: RollingInterval.Day, // Creates a new file daily
                            retainedFileCountLimit: 30, // Optional: limits the number of retained log files
                            fileSizeLimitBytes: 10_000_000 // Optional: limits file size to 10 MB
                        )
                        .CreateLogger();
                    break;
                default:
                    break;
            }
            if (mode == 1)
            {
                
            } 
        }
    }
}
