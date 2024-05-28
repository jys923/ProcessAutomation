using SonoCap.MES.Models;

namespace SonoCap.MES.Services.Converters
{
    public class ProbeTestResultConverter
    {
        public static ProbeTestReport Convert(ProbeTestResult dao)
        {
            ProbeTestReport result = new ProbeTestReport
            {
                Id = dao.Id,
                ProbeSN = dao.ProbeSn,
                CreatedDate = dao.CreatedDate,
                TransducerModuleSN = dao.TransducerModuleSn,
                TransducerSN = dao.TransducerSn,
                MotorModuleSn = dao.MotorModuleSn,
                TestResults = new List<TestResult>()
            };

            // Add TestResults from dao
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId1, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId1, CreatedDate = dao.TestCreatedDate1, Result = dao.TestResult1 });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId2, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId2, CreatedDate = dao.TestCreatedDate2, Result = dao.TestResult2 });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId3, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId3, CreatedDate = dao.TestCreatedDate3, Result = dao.TestResult3 });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId4, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId4, CreatedDate = dao.TestCreatedDate4, Result = dao.TestResult4 });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId5, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId5, CreatedDate = dao.TestCreatedDate5, Result = dao.TestResult5 });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId6, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId6, CreatedDate = dao.TestCreatedDate6, Result = dao.TestResult6 });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId7, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId7, CreatedDate = dao.TestCreatedDate7, Result = dao.TestResult7 });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId8, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId8, CreatedDate = dao.TestCreatedDate8, Result = dao.TestResult8 });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId9, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId9, CreatedDate = dao.TestCreatedDate9, Result = dao.TestResult9 });

            return result;
        }

        public static List<ProbeTestReport> Convert(List<ProbeTestResult> daos)
        {
            var probeTestResults = new List<ProbeTestReport>();

            foreach (var dao in daos)
            {
                var probeTestResult = Convert(dao);

                probeTestResults.Add(probeTestResult);
            }

            return probeTestResults;
        }
    }
}
