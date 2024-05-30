using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoCap.MES.Models.Base;

namespace SonoCap.MES.Models
{
    [Index(nameof(Sn), IsUnique = true)]
    public class Transducer : ModelBase, ISn
    {
        [Required]
        //[StringLength(21)]
        public required string Sn { get; set; }

        [Required]
        public int TransducerTypeId { get; set; }

        [ForeignKey(nameof(TransducerTypeId))] //lazy 로딩
        public virtual TransducerType TransducerType { get; set; } = default!;
    }
}
