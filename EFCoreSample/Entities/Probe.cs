using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreSample.Entities
{
    [Index(nameof(ProbeSN), IsUnique = true)]
    public class Probe : EntityBase
    {
        //[Key]
        //public Guid ProbeId { get; set; }

        [ForeignKey("Id")] //lazy 로딩
        public virtual ProbeType ProbeType { get; set; }

        [Required]
        [StringLength(50)]
        public string ProbeSN { get; set; } = string.Empty;

        //[Required]
        //public Guid InspectId { get; set; }

        //[ForeignKey("Id")] //lazy 로딩
        public virtual List<Inspect> Inspects { get; set; }
    }
}
