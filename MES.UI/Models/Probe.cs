﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MES.UI.Models.Base;

namespace MES.UI.Models
{
    [Index(nameof(ProbeSn), IsUnique = true)]
    public class Probe : ModelBase
    {
        //유니크키
        /// <summary>
        /// SCGP01240303001
        /// </summary>
        [Required]
        //[StringLength(15)]
        public required string ProbeSn { get; set; }

        [Required]
        public required int TransducerModuleId { get; set; }

        [ForeignKey(nameof(TransducerModuleId))] //lazy 로딩
        public virtual TransducerModule TransducerModule { get; set; } = default!;

        [Required]
        public required int MotorModuleId { get; set;}
        
        [ForeignKey(nameof(MotorModuleId))] //lazy 로딩
        public virtual MotorModule MotorModule { get; set; } = default!;
    }
}
