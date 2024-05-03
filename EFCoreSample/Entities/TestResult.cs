using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreSample.Entities
{
    [NotMapped]
    public class TestResult
    {
        public required Enums.TestCategory TestCategoryId { get; set; }
        public required Enums.TestType TestTypeId { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string Result { get; set; } = default!;
    }
}
