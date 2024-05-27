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
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId1, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId1, CreatedDate = dao.Test1_CreatedDate, Result = dao.Test1_Result });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId2, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId2, CreatedDate = dao.Test2_CreatedDate, Result = dao.Test2_Result });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId3, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId3, CreatedDate = dao.Test3_CreatedDate, Result = dao.Test3_Result });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId4, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId4, CreatedDate = dao.Test4_CreatedDate, Result = dao.Test4_Result });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId5, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId5, CreatedDate = dao.Test5_CreatedDate, Result = dao.Test5_Result });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId6, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId6, CreatedDate = dao.Test6_CreatedDate, Result = dao.Test6_Result });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId7, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId7, CreatedDate = dao.Test7_CreatedDate, Result = dao.Test7_Result });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId8, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId8, CreatedDate = dao.Test8_CreatedDate, Result = dao.Test8_Result });
            result.TestResults.Add(new TestResult { CategoryId = (Models.Enums.TestCategories?)dao.TestCategoryId9, TypeId = (Models.Enums.TestTypes?)dao.TestTypeId9, CreatedDate = dao.Test9_CreatedDate, Result = dao.Test9_Result });

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
