﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EFCoreSample.Entities.Enums;

namespace EFCoreSample.Entities
{
    /// <summary>
    /// 로그
    /// </summary>
    public class Test : EntityBase
    {
        [Required]
        public int CategoryId { get; set; }
        
        [ForeignKey(nameof(CategoryId))] //lazy 로딩
        public virtual TestCategory Category { get; set; }

        [Required]
        public int TestTypeId { get; set; }

        [ForeignKey(nameof(TestTypeId))] //lazy 로딩
        public virtual TestType TestType { get; set; }

        [Required]
        public int TesterId { get; set; }

        [ForeignKey(nameof(TesterId))] //lazy 로딩
        public virtual Tester Tester { get; set; }

        [Required]
        public required string OriginalImg { get; set; }

        [Required]
        public required string ChangedImg { get; set; }

        [Required]
        public required string ChangedImgMetadata { get; set; }

        /// <summary>
        /// true > 0 , false = 0
        /// </summary>
        [Required]
        public int Result { get; set; }

        /// <summary>
        /// mode????
        /// auto = 0, manual = 1 검사 방법
        /// </summary>
        [Required]
        public int Method { get; set; }

        [Required]
        public int TransducerModuleId { get; set; }

        [ForeignKey(nameof(TransducerModuleId))] //lazy 로딩
        public virtual TransducerModule TransducerModule { get; set; } // 외부 키의 참조
    }
}