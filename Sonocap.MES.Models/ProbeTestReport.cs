using Microsoft.EntityFrameworkCore;

namespace SonoCap.MES.Models
{
    [Keyless]
    public class ProbeTestReport
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
