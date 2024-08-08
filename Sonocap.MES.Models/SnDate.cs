using SonoCap.MES.Models.Base;

namespace SonoCap.MES.Models
{
    public class SnDate : AddOn
    {
        public string Sn { get; set; } = "";
        public DateTime Date { get; set; }
    }
}
