using System.Diagnostics.CodeAnalysis;

namespace SonoCap.MES.Models.Converts
{
    public class TestToTestProbe
    {
        public static TestProbe Convert(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test), "Test cannot be null");

            // 변환 로직
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
                throw new ArgumentNullException(nameof(tests), "Test cannot be null");

            return tests.Select(test => Convert(test));
        }

        public static async Task<IEnumerable<TestProbe>> ToListAsync(IEnumerable<Test> tests)
        {
            if (tests == null)
                throw new ArgumentNullException(nameof(tests), "Test cannot be null");

            return await Task.Run(() => tests.Select(test => Convert(test)).ToList());
        }
    }
}