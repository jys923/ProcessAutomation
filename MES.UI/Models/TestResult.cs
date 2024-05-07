using MES.UI.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES.UI.Models
{
    [NotMapped]
    public class TestResult
    {
        public required Enums.TestCategory CategoryId { get; set; }
        public required Enums.TestType TypeId { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string Result { get; set; } = default!;
    }
}
