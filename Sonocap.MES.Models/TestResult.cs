using Microsoft.EntityFrameworkCore;
using SonoCap.MES.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoCap.MES.Models
{
    [NotMapped]
    [Keyless]
    public class TestResult
    {
        public TestCategories? CategoryId { get; set; }
        public TestTypes? TypeId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Result { get; set; }
    }
}
