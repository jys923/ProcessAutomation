namespace SonoCap.MES.Models.Api.Dto
{
    public class TestSearchRequest
    {
        // 쿼리 매개변수로 받을 수 있도록 Nullable 타입 사용
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CategoryId { get; set; }
        public int? TestTypeId { get; set; }
        public string? Tester { get; set; }
        public int? PcId { get; set; }
        public int? Result { get; set; }
        public int? DataFlagTest { get; set; }
        public string? ProbeSn { get; set; }
        public string? TransducerModuleSn { get; set; }
        public string? TransducerSn { get; set; }
        public string? MotorModuleSn { get; set; }
        public int? DataFlagProbe { get; set; }
    }
}
