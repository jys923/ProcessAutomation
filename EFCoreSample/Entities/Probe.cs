using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreSample.Entities
{
    [Index(nameof(ProbeSNId), IsUnique = true)]
    public class Probe : EntityBase
    {
        /*[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProbeId { get; set; }*/

        [Required]
        public int ProbeTypeId { get; set; }

        [ForeignKey(nameof(ProbeTypeId))] //lazy 로딩
        public virtual ProbeType ProbeType { get; set; }

        [Required]
        public int ProbeSNId { get; set; }

        [ForeignKey(nameof(ProbeSNId))] //lazy 로딩
        public virtual ProbeSN ProbeSN { get; set; }

        //[Required]
        //public Guid InspectId { get; set; }

        [ForeignKey(nameof(Id))] //lazy 로딩
        public virtual IEnumerable<Inspect> Inspects { get; set; }
    }
}
