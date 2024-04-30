using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace MES.UI.Models
{
    [Index(nameof(ProbeSN), IsUnique = true)]
    [Index(nameof(TransducerModuleSN), IsUnique = true)]
    [Index(nameof(TransducerSN), IsUnique = true)]
    [Index(nameof(MotorModuleSn), IsUnique = true)]
    public class ProbeView : EntityBase
    {
        [Required]
        public required string ProbeSN { get; set; }

        [Required]
        public required string TransducerModuleSN { get; set; }

        [Required]
        public required string TransducerSN { get; set; }

        [Required]
        public required string MotorModuleSn { get; set; }

        [Required]
        public required IEnumerable<ProbeResult> ProbeResults { get; set; }
    }

    [NotMapped]
    public class ProbeResult
    {
        public Enums.TestCategory Category { get; set; }
        public Enums.TestType Type { get; set; }
        public DateTime TestedDate { get; set; }
        public int Result { get; set; }
    }
}
