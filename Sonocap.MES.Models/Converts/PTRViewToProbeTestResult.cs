namespace SonoCap.MES.Models.Converts
{
    public class PTRViewToProbeTestResult
    {
        private static ProbeTestResult Convert(PTRView ptrView)
        {
            if (ptrView == null)
                return null;

            return new ProbeTestResult
            {
                Id = ptrView.Id,
                ProbeSn = ptrView.ProbeSn,
                CreatedDate = ptrView.CreatedDate,
                TransducerModuleSn = ptrView.TransducerModuleSn,
                TransducerSn = ptrView.TransducerSn,
                MotorModuleSn = ptrView.MotorModuleSn,
                TestCategoryId1 = ptrView.Test01.TestCategoryId,
                TestTypeId1 = ptrView.Test01.TestTypeId,
                TestCreatedDate1 = ptrView.Test01.CreatedDate,
                TestResult1 = ptrView.Test01.Result,
                TestCategoryId2 = ptrView.Test02.TestCategoryId,
                TestTypeId2 = ptrView.Test02.TestTypeId,
                TestCreatedDate2 = ptrView.Test02.CreatedDate,
                TestResult2 = ptrView.Test02.Result,
                TestCategoryId3 = ptrView.Test03.TestCategoryId,
                TestTypeId3 = ptrView.Test03.TestTypeId,
                TestCreatedDate3 = ptrView.Test03.CreatedDate,
                TestResult3 = ptrView.Test03.Result,
                TestCategoryId4 = ptrView.Test04.TestCategoryId,
                TestTypeId4 = ptrView.Test04.TestTypeId,
                TestCreatedDate4 = ptrView.Test04.CreatedDate,
                TestResult4 = ptrView.Test04.Result,
                TestCategoryId5 = ptrView.Test05.TestCategoryId,
                TestTypeId5 = ptrView.Test05.TestTypeId,
                TestCreatedDate5 = ptrView.Test05.CreatedDate,
                TestResult5 = ptrView.Test05.Result,
                TestCategoryId6 = ptrView.Test06.TestCategoryId,
                TestTypeId6 = ptrView.Test06.TestTypeId,
                TestCreatedDate6 = ptrView.Test06.CreatedDate,
                TestResult6 = ptrView.Test06.Result,
                TestCategoryId7 = ptrView.Test07?.TestCategoryId,
                TestTypeId7 = ptrView.Test07?.TestTypeId,
                TestCreatedDate7 = ptrView.Test07?.CreatedDate,
                TestResult7 = ptrView.Test07?.Result,
                TestCategoryId8 = ptrView.Test08?.TestCategoryId,
                TestTypeId8 = ptrView.Test08?.TestTypeId,
                TestCreatedDate8 = ptrView.Test08?.CreatedDate,
                TestResult8 = ptrView.Test08?.Result,
                TestCategoryId9 = ptrView.Test09?.TestCategoryId,
                TestTypeId9 = ptrView.Test09?.TestTypeId,
                TestCreatedDate9 = ptrView.Test09?.CreatedDate,
                TestResult9 = ptrView.Test09?.Result,
            };
        }

        public static IEnumerable<ProbeTestResult> ToList(IEnumerable<PTRView> ptrViews)
        {
            if (ptrViews == null)
                return new List<ProbeTestResult>();

            return ptrViews.Select(ptrView => Convert(ptrView));

        }

        public IEnumerable<ProbeTestResult> ToList(List<PTRView> ptrViews)
        {
            if (ptrViews == null)
                return new List<ProbeTestResult>();

            List<ProbeTestResult> probeTestResults = new List<ProbeTestResult>();

            foreach (var ptrView in ptrViews)
            {
                var probeTestResult = new ProbeTestResult
                {
                    Id = ptrView.Id,
                    ProbeSn = ptrView.ProbeSn,
                    CreatedDate = ptrView.CreatedDate,
                    TransducerModuleSn = ptrView.TransducerModuleSn,
                    TransducerSn = ptrView.TransducerSn,
                    MotorModuleSn = ptrView.MotorModuleSn,
                    TestCategoryId1 = ptrView.Test01.TestCategoryId,
                    TestTypeId1 = ptrView.Test01.TestTypeId,
                    TestCreatedDate1 = ptrView.Test01.CreatedDate,
                    TestResult1 = ptrView.Test01.Result,
                    TestCategoryId2 = ptrView.Test02.TestCategoryId,
                    TestTypeId2 = ptrView.Test02.TestTypeId,
                    TestCreatedDate2 = ptrView.Test02.CreatedDate,
                    TestResult2 = ptrView.Test02.Result,
                    TestCategoryId3 = ptrView.Test03.TestCategoryId,
                    TestTypeId3 = ptrView.Test03.TestTypeId,
                    TestCreatedDate3 = ptrView.Test03.CreatedDate,
                    TestResult3 = ptrView.Test03.Result,
                    TestCategoryId4 = ptrView.Test04.TestCategoryId,
                    TestTypeId4 = ptrView.Test04.TestTypeId,
                    TestCreatedDate4 = ptrView.Test04.CreatedDate,
                    TestResult4 = ptrView.Test04.Result,
                    TestCategoryId5 = ptrView.Test05.TestCategoryId,
                    TestTypeId5 = ptrView.Test05.TestTypeId,
                    TestCreatedDate5 = ptrView.Test05.CreatedDate,
                    TestResult5 = ptrView.Test05.Result,
                    TestCategoryId6 = ptrView.Test06.TestCategoryId,
                    TestTypeId6 = ptrView.Test06.TestTypeId,
                    TestCreatedDate6 = ptrView.Test06.CreatedDate,
                    TestResult6 = ptrView.Test06.Result,
                    TestCategoryId7 = ptrView.Test07?.TestCategoryId,
                    TestTypeId7 = ptrView.Test07?.TestTypeId,
                    TestCreatedDate7 = ptrView.Test07?.CreatedDate,
                    TestResult7 = ptrView.Test07?.Result,
                    TestCategoryId8 = ptrView.Test08?.TestCategoryId,
                    TestTypeId8 = ptrView.Test08?.TestTypeId,
                    TestCreatedDate8 = ptrView.Test08?.CreatedDate,
                    TestResult8 = ptrView.Test08?.Result,
                    TestCategoryId9 = ptrView.Test09?.TestCategoryId,
                    TestTypeId9 = ptrView.Test09?.TestTypeId,
                    TestCreatedDate9 = ptrView.Test09?.CreatedDate,
                    TestResult9 = ptrView.Test09?.Result,
                };

                probeTestResults.Add(probeTestResult);
            }

            return probeTestResults;
        }
    }
}
