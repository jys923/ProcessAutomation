using System.Drawing;

namespace SonoCap.MES.UI
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
        public Align Align { get; set; } = default!;
        public Axial Axial { get; set; } = default!;
        public Lateral Lateral { get; set; } = default!;
        public string TesterName { get; set; } = @"No Name";
        public int PcId { get; set; } = 1;
        public MesPath Path { get; set; } = default!;
        public SerilogSettings Serilog { get; set; } = new SerilogSettings();
    }

    public class ConnectionStrings
    {
        public string MariaDBConnection { get; set; } = @"Server=192.168.0.7; Port=3306; Database=sonocap_mes; Uid=root; Pwd=Endolfin12!@;AllowLoadLocalInfile=true;";
    }

    public class Align
    {
        public Circle InnerCircle3 { get; set; } = default!;
        public Circle OuterCircle3 { get; set; } = default!;

        public Circle InnerCircle4 { get; set; } = default!;
        public Circle OuterCircle4 { get; set; } = default!;

        public Circle InnerCircle5 { get; set; } = default!;
        public Circle OuterCircle5 { get; set; } = default!;

        public Circle InnerCircle6 { get; set; } = default!;
        public Circle OuterCircle6 { get; set; } = default!;

        public Circle InnerCircle7 { get; set; } = default!;
        public Circle OuterCircle7 { get; set; } = default!;
    }

    public class Circle
    {
        public int Thickness { get; set; } = 3;
        public int Radius { get; set; } = 170;
        //color
    }

    public class Line
    {
        public int Thickness { get; set; } = 3;
        public Point Start { get; set; }
        public Point End { get; set; }
    }

    public class Axial
    {
        public Line Line3 { get; set; } = default!;
        public Line Line4 { get; set; } = default!;
        public Line Line5 { get; set; } = default!;
        public Line Line6 { get; set; } = default!;
        public Line Line7 { get; set; } = default!;
    }

    public class Lateral
    {
        public Line Line3 { get; set; } = default!;
        public Line Line4 { get; set; } = default!;
        public Line Line5 { get; set; } = default!;
        public Line Line6 { get; set; } = default!;
        public Line Line7 { get; set; } = default!;
    }

    public class MesPath
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