namespace SonoCap.MES.Models
{
    public class ExportTest
    {
        public DateTime Date { get; set; } = default!;                    // 검사일시
        public string Tester { get; set; } = "";                          // 검사자
        public string TestCategory { get; set; } = "";                    // 검사 유형 (공정, 최종 등)
        public string TestType { get; set; } = "";                        // 검사 항목 (Gray, Res 등)
        public string Method { get; set; } = "";                          // 검사 방식 (auto/manual/force 등)
        public string Result { get; set; } = "";                          // 결과 (PASS/FAIL)

        public string ProbeSn { get; set; } = "";                         // 프로브 S/N
        public string TransducerSn { get; set; } = "";                    // 트랜스듀서 S/N
        public string TransducerModuleSn { get; set; } = default!;        // TD 모듈 S/N
        public string MotorModuleSn { get; set; } = default!;             // MT 모듈 S/N

        public string OriginalImg { get; set; } = "";                     // 원본 이미지 경로
        public string ChangedImg { get; set; } = "";                      // 검사 이미지 경로
        public string Metadata { get; set; } = "";                        // 메타데이터
    }
}
