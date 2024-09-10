using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;

namespace SonoCap.Tests.MES.Repositories.Base
{
    public static class TestHelper
    {
        private static TransducerType transducerType1 = new TransducerType { Code = "01", Type="001" };
        private static TransducerType transducerType2 = new TransducerType { Code = "02", Type = "002" };
        private static TestCategory testCategory1 = new TestCategory { Name = "Category1" };
        private static TestCategory testCategory2 = new TestCategory { Name = "Category2" };
        private static TestCategory testCategory3 = new TestCategory { Name = "Category3" };
        private static TestType testType1 = new TestType { Name = "Type1", Threshold = 91 };
        private static TestType testType2 = new TestType { Name = "Type2", Threshold = 92 };
        private static TestType testType3 = new TestType { Name = "Type3", Threshold = 93 };
        private static Pc pc1 = new Pc { Name = "PC1" };
        private static Pc pc2 = new Pc { Name = "PC2" };
        private static Pc pc3 = new Pc { Name = "PC3" };
        private static Tester tester1 = new Tester { Name = "Tester1", Pc = pc1 };
        private static Tester tester2 = new Tester { Name = "Tester2", Pc = pc2 };
        private static Tester tester3 = new Tester { Name = "Tester3", Pc = pc3 };

        public static void SeedDatabase(MESDbContext context)
        {
            var transducer = new Transducer { Sn = "TransducerSN" };
            var motorModule = new MotorModule { Sn = "MotorModuleSN" };
            var transducerModule = new TransducerModule { Sn = "TransducerModuleSN" };
            var probe = new Probe { Sn = "ProbeSN", MotorModuleId = 1, TransducerModuleId = 1 };

            context.TestCategories.Add(testCategory1);
            context.TestTypes.Add(testType1);
            context.Testers.Add(tester1);
            context.Transducers.Add(transducer);
            context.MotorModules.Add(motorModule);
            context.TransducerModules.Add(transducerModule);
            context.Probes.Add(probe);

            var test = new Test
            {
                CreatedDate = DateTime.Now,
                DataFlag = 1,
                TestCategoryId = testCategory1.Id,
                TestTypeId = testType1.Id,
                TesterId = tester1.Id,
                ProbeId = probe.Id,
                TransducerModuleId = transducerModule.Id,
                TransducerId = transducer.Id,
                Result = 10
            };

            context.Tests.Add(test);
            context.SaveChanges();
        }

        public static void SeedDatabaseForDetailedTests(MESDbContext context)
        {
            context.TransducerTypes.Add(transducerType1);
            context.TransducerTypes.Add(transducerType2);
            context.TestCategories.Add(testCategory1);
            context.TestCategories.Add(testCategory2);
            context.TestCategories.Add(testCategory3);
            context.TestTypes.Add(testType1);
            context.TestTypes.Add(testType2);
            context.TestTypes.Add(testType3);
            context.Testers.Add(tester1);
            context.Testers.Add(tester2);
            context.Testers.Add(tester3);

            context.SaveChanges();
        }

        public static void SeedDatabaseForDetailedTestsProbe(MESDbContext context)
        {
            var transducer1 = new Transducer { Sn = "TransducerSN1", TransducerTypeId = 1 };
            context.Transducers.Add(transducer1);
            var test1 = new Test { TestCategoryId = 1, TestTypeId = 1, TesterId = 1, Result = 90, TransducerId = 1};
            var test2 = new Test { TestCategoryId = 1, TestTypeId = 2, TesterId = 1, Result = 91, TransducerId = 1};
            var test3 = new Test { TestCategoryId = 1, TestTypeId = 3, TesterId = 1, Result = 92, TransducerId = 1};
            context.Tests.Add(test1);
            context.Tests.Add(test2);
            context.Tests.Add(test3);
            var transducerModule1 = new TransducerModule { Sn = "TransducerModuleSN1", TransducerId=1 };
            context.TransducerModules.Add(transducerModule1);
            var test4 = new Test { TestCategoryId = 2, TestTypeId = 1, TesterId = 1, Result = 90, TransducerModuleId = 1 };
            var test5 = new Test { TestCategoryId = 2, TestTypeId = 2, TesterId = 1, Result = 91, TransducerModuleId = 1 };
            var test6 = new Test { TestCategoryId = 2, TestTypeId = 3, TesterId = 1, Result = 92, TransducerModuleId = 1 };
            context.Tests.Add(test4);
            context.Tests.Add(test5);
            context.Tests.Add(test6);
            var motorModule1 = new MotorModule { Sn = "MotorModuleSN1" };
            context.MotorModules.Add(motorModule1);
            var probe1 = new Probe { Sn = "ProbeSN1", MotorModuleId = 1, TransducerModuleId = 1 };
            context.Probes.Add(probe1);
            var test7 = new Test { TestCategoryId = 3, TestTypeId = 1, TesterId = 1, Result = 90, ProbeId = 1 };
            var test8 = new Test { TestCategoryId = 3, TestTypeId = 2, TesterId = 1, Result = 91, ProbeId = 1 };
            var test9 = new Test { TestCategoryId = 3, TestTypeId = 3, TesterId = 1, Result = 92, ProbeId = 1 };
            context.Tests.Add(test7);
            context.Tests.Add(test8);
            context.Tests.Add(test9);
            context.SaveChanges();
        }
    }
}
