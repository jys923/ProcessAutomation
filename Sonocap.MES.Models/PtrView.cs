﻿using SonoCap.MES.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SonoCap.MES.Models
{
    [Index(nameof(ProbeSn), IsUnique = true)]
    [Index(nameof(TransducerModuleSn), IsUnique = true)]
    [Index(nameof(TransducerSn), IsUnique = true)]
    public class PTRView : ModelBase
    {
        [Required]
        public required string ProbeSn { get; set; }
        
        [Required]
        public required string TransducerModuleSn { get; set; }
        
        [Required]
        public required string TransducerSn { get; set; }
        
        [Required]
        public required string MotorModuleSn { get; set; }

        [Required]
        public required int TestId01 { get; set; }
        [Required]
        public required int TestId02 { get; set; }
        [Required]
        public required int TestId03 { get; set; }

        [Required]
        public required int TestId04 { get; set; }
        [Required]
        public required int TestId05 { get; set; }
        [Required]
        public required int TestId06 { get; set; }

        public int? TestId07 { get; set; }
        public int? TestId08 { get; set; }
        public int? TestId09 { get; set; }

        [ForeignKey(nameof(TestId01))]
        public virtual Test Test01 { get; set; } = default!;

        [ForeignKey(nameof(TestId02))]
        public virtual Test Test02 { get; set; } = default!;

        [ForeignKey(nameof(TestId03))]
        public virtual Test Test03 { get; set; } = default!;

        [ForeignKey(nameof(TestId04))]
        public virtual Test Test04 { get; set; } = default!;

        [ForeignKey(nameof(TestId05))]
        public virtual Test Test05 { get; set; } = default!;

        [ForeignKey(nameof(TestId06))]
        public virtual Test Test06 { get; set; } = default!;

        [ForeignKey(nameof(TestId07))]
        public virtual Test? Test07 { get; set; }

        [ForeignKey(nameof(TestId08))]
        public virtual Test? Test08 { get; set; }

        [ForeignKey(nameof(TestId09))]
        public virtual Test? Test09 { get; set; }
    }
}
