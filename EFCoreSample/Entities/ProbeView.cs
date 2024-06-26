﻿using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreSample.Entities
{
    [NotMapped]
    public class ProbeView
    {
        public int Id { get; set; }
        public required string ProbeSN { get; set; }
        public DateTime CreatedDate { get; set; }
        public required string TransducerModuleSN { get; set; }
        public required string TransducerSN { get; set; }
        public required string MotorModuleSn { get; set; }

        // 카테고리 1 테스트 결과
        public required List<TestResult> Category1Results { get; set; } = new List<TestResult>();

        // 카테고리 2 테스트 결과
        public required List<TestResult> Category2Results { get; set; } = new List<TestResult>();
    }
}
