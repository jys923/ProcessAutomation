using SonoCap.MES.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoCap.MES.Models
{
    [NotMapped]
    [Keyless]
    public class TestResult
    {
        public SonoCap.MES.Models.Enums.TestCategories? CategoryId { get; set; }
        public SonoCap.MES.Models.Enums.TestTypes? TypeId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Result { get; set; } = default!;
    }
}
