using SonoCap.MES.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoCap.MES.Models
{
    public class Test : ModelBase
    {
        [Required]
        public int TestCategoryId { get; set; }

        [ForeignKey(nameof(TestCategoryId))] //lazy 로딩
        public virtual TestCategory TestCategory { get; set; } = default!;

        [Required]
        public int TestTypeId { get; set; }

        [ForeignKey(nameof(TestTypeId))] //lazy 로딩
        public virtual TestType TestType { get; set; } = default!;

        [Required]
        public int TesterId { get; set; }

        [ForeignKey(nameof(TesterId))] //lazy 로딩
        public virtual Tester Tester { get; set; } = default!;

        [Required]
        public string OriginalImg { get; set; } = "";

        [Required]
        public string ChangedImg { get; set; } = "";

        [Required]
        public string ChangedImgMetadata { get; set; } = "";

        /// <summary>
        /// true > 0 , false = 0
        /// </summary>
        [Required]
        public int Result { get; set; }

        /// <summary>
        /// mode????
        /// auto = 0, manual = 1 검사 방법
        /// </summary>
        [Required]
        public int Method { get; set; }

        public int? TransducerId { get; set; }

        [ForeignKey(nameof(TransducerId))] //lazy 로딩
        public virtual Transducer? Transducer { get; set; }

        public int? TransducerModuleId { get; set; }

        [ForeignKey(nameof(TransducerModuleId))] //lazy 로딩
        public virtual TransducerModule? TransducerModule { get; set; }

        public int? ProbeId { get; set; }

        [ForeignKey(nameof(ProbeId))] //lazy 로딩
        public virtual Probe? Probe { get; set; }
    }
}