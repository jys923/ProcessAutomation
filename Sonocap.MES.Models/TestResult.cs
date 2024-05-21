using SonoCap.MES.Models.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoCap.MES.Models
{
    [NotMapped]
    [Keyless]
    public class TestResult
    {
        public Enums.TestCategory? CategoryId { get; set; }
        public Enums.TestType? TypeId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Result { get; set; } = default!;
    }
}
