using Microsoft.EntityFrameworkCore;
using SonoCap.MES.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SonoCap.MES.Models
{
    public class ProbeTestResultView : ModelBase
    {
        public required string ProbeSn { get; set; }
        public required string TransducerModuleSn { get; set; }
        public required string TransducerSn { get; set; }
        public required string MotorModuleSn { get; set; }

        [Required]
        public required int TestId01 { get; set; }
        [Required]
        public required int TestId02 { get; set; }
        [Required]
        public required int TestId03 { get; set; }

        [Required]
        public required int TestId04 { get; set; }
        [Required]
        public required int TestId05 { get; set; }
        [Required]
        public required int TestId06 { get; set; }

        public int? TestId07 { get; set; }
        public int? TestId08 { get; set; }
        public int? TestId09 { get; set; }

        [ForeignKey(nameof(TestId01))]
        public virtual Test Test01 { get; set;}

        [ForeignKey(nameof(TestId02))]
        public virtual Test Test02 { get; set;}

        [ForeignKey(nameof(TestId03))]
        public virtual Test Test03 { get; set;}

        [ForeignKey(nameof(TestId04))]
        public virtual Test Test04 { get; set;}

        [ForeignKey(nameof(TestId05))]
        public virtual Test Test05 { get; set;}

        [ForeignKey(nameof(TestId06))]
        public virtual Test Test06 { get; set;}

        [ForeignKey(nameof(TestId07))]
        public virtual Test Test07 { get; set;}

        [ForeignKey(nameof(TestId08))]
        public virtual Test Test08 { get; set;}

        [ForeignKey(nameof(TestId09))]
        public virtual Test Test09 { get; set;}     
    }
}
