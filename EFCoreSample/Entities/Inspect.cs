using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreSample.Entities
{
    public class Inspect : EntityBase
    {
        //[Key]
        //public Guid InspectId { get; set; }

        [Required]
        public int InspectTypeId { get; set; }

        [ForeignKey("InspectTypeId")] //lazy 로딩
        public virtual InspectType InspectType { get; set; }

        [Required]
        [StringLength(10)]
        public string InspectorName { get; set; } = string.Empty;

        [Required]
        public int InspectPcNo { get; set; }

        [Required]
        public string OriginalImg { get; set; } = string.Empty;

        [Required]
        public string ChangedImg { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string ChangedImgMetadata { get; set; } = string.Empty;

        [Required]
        public int ProbeId { get; set; } // 외부 키

        [ForeignKey("ProbeId")] //lazy 로딩
        public virtual Probe Probe { get; set; } // 외부 키의 참조
    }
}