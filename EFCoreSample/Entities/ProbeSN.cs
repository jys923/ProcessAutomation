using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System;

namespace EFCoreSample.Entities
{
    [Index(nameof(ProbeSn), IsUnique = true)]
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
        [StringLength(21)]
        public required string ProbeSn { get; set; }
        //[ForeignKey(nameof(Id))] //lazy 로딩
        //public virtual IEnumerable<Inspect> Inspects { get; set; }
    }
}
