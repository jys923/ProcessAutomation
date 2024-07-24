namespace SonoCap.MES.UI
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = default!;
        public Circle Circle { get; set; } = default!;
        public string TesterName { get; set; } = @"No Name";
        public int PcId { get; set; } = 1;
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
}