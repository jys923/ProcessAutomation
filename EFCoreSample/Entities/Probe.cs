using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSample.Entities
{
    [Index(nameof(ProbeSn), IsUnique = true)]
    public class Probe : EntityBase
    {
        //유니크키
        /// <summary>
        /// SCGP01240303001
        /// </summary>
        [Required]
        //[StringLength(15)]
        public required string ProbeSn { get; set; }

        [Required]
        public int TransducerModuleId { get; set; }

        [ForeignKey(nameof(TransducerModuleId))] //lazy 로딩
        public virtual TransducerModule TransducerModule { get; set; } // 외부 키의 참조

        [Required]
        public int MotorModuleId { get; set;}
        
        [ForeignKey(nameof(MotorModuleId))] //lazy 로딩
        public virtual MotorModule MotorModule { get; set; } // 외부 키의 참조
    }
}
