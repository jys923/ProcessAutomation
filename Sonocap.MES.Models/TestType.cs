﻿using SonoCap.MES.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace SonoCap.MES.Models
{
    /// <summary>
    /// 1번 검사 2번 검사...
    /// </summary>
    public class TestType : ModelBase
    {
        [Required]
        [StringLength(10)]
        public required string Name { get; set; }

        [Required]
        public int Threshold { get; set; } = new Random().Next(70, 100);
    }
}