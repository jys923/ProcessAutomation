using System.Runtime;

namespace SonoCap.Commons
{
    public class AppSettings
    {
        public AppSettings()
        {
            ConnectionStrings = new ConnectionStrings();
            Path = new MesPath();
            Serilog = new SerilogSettings();
            DbSettings = new DbSettings(); // ← 추가
        }

        public ConnectionStrings ConnectionStrings { get; set; }
        public string TesterName { get; set; } = "No Name";
        public int PcId { get; set; } = 1;
        public MesPath Path { get; set; }
        public SerilogSettings Serilog { get; set; }
        public DbSettings DbSettings { get; set; } // ← 추가
    }

    public class DbSettings
    {
        public bool AutoMigrate { get; set; } = false;
    }

    public class ConnectionStrings
    {
        public string MariaDBConnection { get; set; } = @"Server=localhost; Port=3306; Database=sonocap_mes; Uid=root; Pwd=Endolfin12!@;AllowLoadLocalInfile=true;Connection Timeout=5;";
    }

    public class MesPath
    {
        public string ExportExcel { get; set; } = @"./report/";
        //public string ExportImg { get; set; } = @"./img/";
        public string ExportImg { get; set; } = @"./capture/images/";
        public string ExportVideo { get; set; } = @"./capture/videos/";

        public Dictionary<string, string> ExportImgPhase { get; set; } = new()
        {
            { "Process", "공정" },
            { "Product", "완제품" },
            { "Final", "최종" }
        };
    }

    public class SerilogSettings
    {
        public string[] Using { get; set; } = new[] { "Serilog.Sinks.Console", "Serilog.Sinks.File" };

        public MinimumLevel MinimumLevel { get; set; } = new();

        public WriteTo[] WriteTo { get; set; } = new[]
        {
        new WriteTo
        {
            Name = "Console",
            Args = new ConsoleArgs
            {
                OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            }
        },
        new WriteTo
        {
            Name = "File",
            Args = new FileArgs
            {
                Path = "logs/log-.txt",
                OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                RollingInterval = "Day",
                RetainedFileCountLimit = 30,
                FileSizeLimitBytes = 10000000
            }
        }
    };
    }

    public class MinimumLevel
    {
        public string Default { get; set; } = "Information";
        public Dictionary<string, string>? Override { get; set; }
    }

    public class WriteTo
    {
        public string Name { get; set; } = string.Empty;
        public object Args { get; set; } = default!;
    }

    public class FileArgs
    {
        public string Path { get; set; } = string.Empty;
        public string OutputTemplate { get; set; } = string.Empty;
        public string RollingInterval { get; set; } = "Day";
        public int RetainedFileCountLimit { get; set; }
        public int FileSizeLimitBytes { get; set; }
    }

    public class ConsoleArgs
    {
        public string OutputTemplate { get; set; } = string.Empty;
    }
}