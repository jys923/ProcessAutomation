using MES.UI.Models.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES.UI.Models
{
    [NotMapped]
    [Keyless]
    public class TestResult
    {
        public required Enums.TestCategory CategoryId { get; set; }
        public required Enums.TestType TypeId { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string Result { get; set; } = default!;
    }
}
