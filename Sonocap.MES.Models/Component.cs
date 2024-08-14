using SonoCap.MES.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoCap.MES.Models
{
    public class Component : ModelBase
    {
        public int? TransducerId { get; set; }
        public int? TransducerModuleId { get; set; }
        public int? ProbeId { get; set; }

        [ForeignKey(nameof(TransducerId))] //lazy 로딩
        public virtual Transducer? Transducer { get; set; }

        [ForeignKey(nameof(TransducerModuleId))] //lazy 로딩
        public virtual TransducerModule? TransducerModule { get; set; }

        [ForeignKey(nameof(ProbeId))] //lazy 로딩
        public virtual Probe? Probe { get; set; }
    }
}
