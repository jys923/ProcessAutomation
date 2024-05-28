using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoCap.MES.Models
{
    [Keyless]
    public class TestProbe
    {
        public required int Id { get; set; }
        public required DateTime CreatedDate { get; set; }
        public string? Detail { get; set; }
        public required virtual TestCategory Category { get; set; }
        public required virtual TestType TestType { get; set; }
        public required string Tester { get; set; }
        public required virtual Pc Pc { get; set; }
        public required string OriginalImg { get; set; }
        public required string ChangedImg { get; set; }
        public required string ChangedImgMetadata { get; set; }
        public int Result { get; set; }
        public int Method { get; set; }

        public virtual Probe? Probe { get; set; }

        public virtual TransducerModule? TransducerModule { get; set; } = default!;

        public virtual Transducer? Transducer { get; set; } = default!;

        public virtual MotorModule? MotorModule { get; set; }
    }
}
