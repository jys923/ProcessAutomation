using System.ComponentModel.DataAnnotations.Schema;

namespace MES.UI.Models
{
    [NotMapped]
    public class TransducerModuleView
    {
        public required string TransducerModuleSn { get; set; }
        public required IList<int> Results { get; set; }
    }
}
