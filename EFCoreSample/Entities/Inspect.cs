using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreSample.Entities
{
    /// <summary>
    /// 로그
    /// </summary>
    public class Inspect : EntityBase
    {
        /*[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InspectId { get; set; }*/

        [Required]
        public int InspectTypeId { get; set; }

        [ForeignKey(nameof(InspectTypeId))] //lazy 로딩
        public virtual InspectType InspectType { get; set; }

        [Required]
        public int InspectorTypeId { get; set; }

        [ForeignKey(nameof(InspectorTypeId))] //lazy 로딩
        public virtual InspectorType InspectorType { get; set; }

        [Required]
        public required string OriginalImg { get; set; }

        [Required]
        public required string ChangedImg { get; set; }

        [Required]
        public required string ChangedImgMetadata { get; set; }

        /// <summary>
        /// true = 1 , false = 0
        /// </summary>
        [Required]
        public int Result { get; set; }

        /// <summary>
        /// auto = 0, manual = 1 검사 방법
        /// </summary>
        [Required]
        public int Method { get; set; }

        [Required]
        public int ProbeSNId { get; set; }

        [ForeignKey(nameof(ProbeSNId))] //lazy 로딩
        public virtual ProbeSN ProbeSN { get; set; } // 외부 키의 참조
    }
}