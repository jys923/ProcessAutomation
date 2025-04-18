using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace SonoCap.Commons
{
    public static class LoggingConfigurator
    {
        public static void Configure(SerilogSettings settings)
        {
            var loggerConfig = new LoggerConfiguration();

            // 최소 로그 레벨
            if (!string.IsNullOrEmpty(settings.MinimumLevel?.Default))
            {
                var level = Enum.Parse<Serilog.Events.LogEventLevel>(settings.MinimumLevel.Default, true);
                loggerConfig.MinimumLevel.Is(level);
            }

            // WriteTo 설정
            foreach (var target in settings.WriteTo)
            {
                switch (target.Name.ToLower())
                {
                    case "console":
                        if (target.Args is ConsoleArgs consoleArgs)
                        {
                            loggerConfig.WriteTo.Console(
                                outputTemplate: consoleArgs.OutputTemplate,
                                theme: AnsiConsoleTheme.Code);
                        }
                        break;

                    case "file":
                        if (target.Args is FileArgs fileArgs)
                        {
                            loggerConfig.WriteTo.File(
                                path: fileArgs.Path,
                                outputTemplate: fileArgs.OutputTemplate,
                                rollingInterval: Enum.Parse<RollingInterval>(fileArgs.RollingInterval, true),
                                retainedFileCountLimit: fileArgs.RetainedFileCountLimit,
                                fileSizeLimitBytes: fileArgs.FileSizeLimitBytes
                            );
                        }
                        break;
                }
            }

            Log.Logger = loggerConfig.CreateLogger();
        }
    }
}
