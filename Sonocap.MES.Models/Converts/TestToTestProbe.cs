namespace SonoCap.MES.Models.Converts
{
    public class TestToTestProbe
    {
        public static TestProbe Convert(Test test)
        {
            if (test == null)
                return null;

            return new TestProbe
            {
                Id = test.Id,
                CreatedDate = test.CreatedDate,
                Detail = test.Detail,
                Category = test.TestCategory,
                TestType = test.TestType,
                Tester = test.Tester.Name,
                Pc = test.Tester.Pc,
                OriginalImg = test.OriginalImg,
                ChangedImg = test.ChangedImg,
                ChangedImgMetadata = test.ChangedImgMetadata,
                Result = test.Result,
                Method = test.Method,
                Probe = test.Probe,
                TransducerModule = test.TransducerModule,
                Transducer = test.Transducer,
                MotorModule = test.Probe?.MotorModule??null
            };
        }

        public static IEnumerable<TestProbe> ToList(IEnumerable<Test> tests)
        {
            if (tests == null)
                return new List<TestProbe>();

            return tests.Select(test => Convert(test));

        }
    }

}
