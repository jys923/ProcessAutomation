using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoCap.MES.UI
{
    public class AppSettings
    {
        public ConnectionStrings? ConnectionStrings { get; set; }
        public Circle? Circle { get; set; }
    }

    public class ConnectionStrings
    {
        public string? MariaDBConnection { get; set; }
    }

    public class Circle
    {
        public int? Depth3 { get; set; }
        public int? Depth4 { get; set; }
        public int? Depth5 { get; set; }
        public int? Depth6 { get; set; }
        public int? Depth7 { get; set; }
    }
}
