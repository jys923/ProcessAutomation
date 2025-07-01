using System;
using SonoCap.MES.Models;

namespace SonoCap.MES.Converters
{
    public static class TestToExportTest
    {
        public static ExportTest Convert(Test test)
        {
            return new ExportTest
            {
                Date = test.CreatedDate,
                Tester = test.Tester?.Name ?? "",
                TestCategory = test.TestCategory?.Name ?? "",
                TestType = test.TestType?.Name ?? "",
                Method = ConvertMethod(test.Method),
                Result = ConvertResult(test.Result),

                ProbeSn = test.Probe?.Sn ?? "",
                TransducerSn = test.Transducer?.Sn ?? "",
                TransducerModuleSn = test.TransducerModule?.Sn ?? "",
                MotorModuleSn = test.Probe?.MotorModule.Sn ?? "",

                OriginalImg = test.OriginalImg,
                ChangedImg = test.ChangedImg,
                Metadata = test.ChangedImgMetadata
            };
        }

        private static string ConvertMethod(int method)
        {
            return method switch
            {
                0 => "Auto",
                1 => "Manual",
                2 => "Force",
                _ => "Unknown"
            };
        }

        private static string ConvertResult(int result)
        {
            return result > 0 ? "PASS" : "FAIL";
        }
    }
}
