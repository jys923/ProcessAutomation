using Microsoft.EntityFrameworkCore;

namespace SonoCap.MES.Models
{
    [Keyless]
    public class ProbeTestResultDao
    {
        public required int Id { get; set; }
        public required string ProbeSn { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string TransducerModuleSn { get; set; }
        public required string TransducerSn { get; set; }
        public required string MotorModuleSn { get; set; }

        public int? TestCategoryId1 { get; set; }
        public int? TestTypeId1 { get; set; }
        public DateTime? Test1_CreatedDate { get; set; }
        public int? Test1_Result { get; set; }

        public int? TestCategoryId2 { get; set; }
        public int?  TestTypeId2 { get; set; }
        public DateTime? Test2_CreatedDate { get; set; }
        public int? Test2_Result { get; set; }

        public int? TestCategoryId3 { get; set; }
        public int? TestTypeId3 { get; set; }
        public DateTime? Test3_CreatedDate { get; set; }
        public int? Test3_Result { get; set; }

        public int? TestCategoryId4 { get; set; }
        public int? TestTypeId4 { get; set; }
        public DateTime? Test4_CreatedDate { get; set; }
        public int? Test4_Result { get; set; }

        public int? TestCategoryId5 { get; set; }
        public int? TestTypeId5 { get; set; }
        public DateTime? Test5_CreatedDate { get; set; }
        public int? Test5_Result { get; set; }

        public int? TestCategoryId6 { get; set; }
        public int? TestTypeId6 { get; set; }
        public DateTime? Test6_CreatedDate { get; set; }
        public int? Test6_Result { get; set; }

        public int? TestCategoryId7 { get; set; }
        public int? TestTypeId7 { get; set; }
        public DateTime? Test7_CreatedDate { get; set; }
        public int? Test7_Result { get; set; }

        public int? TestCategoryId8 { get; set; }
        public int? TestTypeId8 { get; set; }
        public DateTime? Test8_CreatedDate { get; set; }
        public int? Test8_Result { get; set; }

        public int? TestCategoryId9 { get; set; }
        public int? TestTypeId9 { get; set; }
        public DateTime? Test9_CreatedDate { get; set; }
        public int? Test9_Result { get; set; }
    }

}
