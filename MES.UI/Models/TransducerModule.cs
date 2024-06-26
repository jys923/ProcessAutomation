﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MES.UI.Models.Base;

namespace MES.UI.Models
{
    [Index(nameof(TransducerSn), IsUnique = true)]
    [Index(nameof(TransducerModuleSn), IsUnique = true)]
    public class TransducerModule : ModelBase
    {
        [Required]
        //[StringLength(21)]
        public required string TransducerModuleSn { get; set; }

        [Required]
        //[StringLength(21)]
        public required string TransducerSn { get; set; }

        [Required]
        public int TransducerTypeId { get; set; }

        [ForeignKey(nameof(TransducerTypeId))] //lazy 로딩
        public virtual TransducerType TransducerType { get; set; } = default!;

        //[ForeignKey(nameof(Id))] //lazy 로딩
        //public virtual IEnumerable<Test> Tests { get; set; }
    }
}
