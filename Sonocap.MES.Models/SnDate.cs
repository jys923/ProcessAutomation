using SonoCap.MES.Models.Base;

namespace SonoCap.MES.Models
{
    public class SnDate : AddOn
    {
        public required string Sn { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string? Type { get; set; }
    }
}
