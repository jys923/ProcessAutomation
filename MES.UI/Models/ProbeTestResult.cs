using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES.UI.Models
{
    [Keyless]
    public class ProbeTestResult
    {
        public required int Id { get; set; }
        public required string ProbeSN { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required string TransducerModuleSN { get; set; }
        public required string TransducerSN { get; set; }
        public required string MotorModuleSn { get; set; }
        public required List<TestResult> TestResults { get; set; }
    }
}
