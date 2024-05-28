using Microsoft.EntityFrameworkCore;

namespace SonoCap.MES.Models
{
    [Keyless]
    public class ProbeTestResult
    {
        public required int Id { get; set; }
        public required string ProbeSn { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string TransducerModuleSn { get; set; }
        public required string TransducerSn { get; set; }
        public required string MotorModuleSn { get; set; }

        public int? TestCategoryId1 { get; set; }
        public int? TestTypeId1 { get; set; }
        public DateTime? TestCreatedDate1 { get; set; }
        public int? TestResult1 { get; set; }

        public int? TestCategoryId2 { get; set; }
        public int?  TestTypeId2 { get; set; }
        public DateTime? TestCreatedDate2 { get; set; }
        public int? TestResult2 { get; set; }

        public int? TestCategoryId3 { get; set; }
        public int? TestTypeId3 { get; set; }
        public DateTime? TestCreatedDate3 { get; set; }
        public int? TestResult3 { get; set; }

        public int? TestCategoryId4 { get; set; }
        public int? TestTypeId4 { get; set; }
        public DateTime? TestCreatedDate4 { get; set; }
        public int? TestResult4 { get; set; }

        public int? TestCategoryId5 { get; set; }
        public int? TestTypeId5 { get; set; }
        public DateTime? TestCreatedDate5 { get; set; }
        public int? TestResult5 { get; set; }

        public int? TestCategoryId6 { get; set; }
        public int? TestTypeId6 { get; set; }
        public DateTime? TestCreatedDate6 { get; set; }
        public int? TestResult6 { get; set; }

        public int? TestCategoryId7 { get; set; }
        public int? TestTypeId7 { get; set; }
        public DateTime? TestCreatedDate7 { get; set; }
        public int? TestResult7 { get; set; }

        public int? TestCategoryId8 { get; set; }
        public int? TestTypeId8 { get; set; }
        public DateTime? TestCreatedDate8 { get; set; }
        public int? TestResult8 { get; set; }

        public int? TestCategoryId9 { get; set; }
        public int? TestTypeId9 { get; set; }
        public DateTime? TestCreatedDate9 { get; set; }
        public int? TestResult9 { get; set; }
    }

}
