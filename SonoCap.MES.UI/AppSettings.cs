namespace SonoCap.MES.UI
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
        public Circle Circle { get; set; } = default!;
        public string TesterName { get; set; } = @"No Name";
        public int PcId { get; set; } = 1;
        public Path Path { get; set; } = default!;
        public SerilogSettings Serilog { get; set; } = new SerilogSettings();
    }

    public class ConnectionStrings
    {
        public string MariaDBConnection { get; set; } = @"Server=192.168.0.61; Port=3306; Database=sonocap_mes; Uid=root; Pwd=Endolfin12!@;AllowLoadLocalInfile=true;";
    }

    public class Circle
    {
        public int Depth3 { get; set; } = 170;
        public int Depth4 { get; set; } = 160;
        public int Depth5 { get; set; } = 150;
        public int Depth6 { get; set; } = 140;
        public int Depth7 { get; set; } = 130;
    }

    public class Path
    {
        public string ImportExcel { get; set; } = @"./report/";
        public string ExportExcel { get; set; } = @"./report/";
        public string ExportImg { get; set; } = @"./img/";
    }

    public class SerilogSettings
    {
        public MinimumLevel MinimumLevel { get; set; } = new MinimumLevel();
        public WriteTo[] WriteTo { get; set; } = new[]
        {
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
    }

    public class WriteTo
    {
        public string Name { get; set; } = "File";
        public FileArgs Args { get; set; } = new FileArgs();
    }

    public class FileArgs
    {
        public string Path { get; set; } = "logs/log-.txt";
        public string OutputTemplate { get; set; } = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        public string RollingInterval { get; set; } = "Day";
        public int? RetainedFileCountLimit { get; set; } = 30;
        public long? FileSizeLimitBytes { get; set; } = 10000000;
    }
}