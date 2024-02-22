using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.Entities
{
    public class ProbeSN : EntityBase
    {
        /*[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProbeSNId { get; set; }*/

        [Required]
        public int ProbeTypeId { get; set; }

        [ForeignKey(nameof(ProbeTypeId))] //lazy 로딩
        public virtual ProbeType ProbeType { get; set; }

        [Required]
        public int ProbeSNTypeId { get; set; }

        [ForeignKey(nameof(ProbeSNTypeId))] //lazy 로딩
        public virtual ProbeSNType ProbeSNType { get; set; }

        /// <summary>
        /// 시리얼 마지막 20240216 A PC에서 1번 제품
        /// </summary>
        [Required]
        public int ProbeSeqNo { get; set; }

        //[ForeignKey(nameof(Id))] //lazy 로딩
        //public virtual IEnumerable<Inspect> Inspects { get; set; }
    }
}
