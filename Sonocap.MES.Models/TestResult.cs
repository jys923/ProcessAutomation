using Microsoft.EntityFrameworkCore;
using SonoCap.MES.Models.Enums;

namespace SonoCap.MES.Models
{
    [Keyless]
    public class TestResult
    {
        public TestCategories? CategoryId { get; set; }
        public TestTypes? TypeId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Result { get; set; }
    }
}
