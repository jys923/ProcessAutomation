using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoCap.MES.Models.Base;

namespace SonoCap.MES.Models
{
    [Index(nameof(TransducerModuleSn), IsUnique = true)]
    public class TransducerModule : ModelBase
    {
        [Required]
        //[StringLength(21)]
        public required string TransducerModuleSn { get; set; }

        [Required]
        public int TransducerId { get; set; }

        [ForeignKey(nameof(TransducerId))] //lazy 로딩
        public virtual Transducer Transducer { get; set; } = default!;

        // Navigation properties
        public virtual ICollection<ProbeTestResultView> PTRViewTransducerModule { get; set; }
    }
}
