namespace SonoCap.MES.Models.Converts.Tests
{
    public class TestToTestProbeTests
    {
        private TestCategory testCategory;
        private TestType testType;
        private Tester tester;
        private Probe probe;
        private TransducerModule transducerModule;
        private Transducer transducer;
        private MotorModule motorModule;

        public TestToTestProbeTests()
        {
            testCategory = new TestCategory { Id = 1, Name = "Category1" };
            testType = new TestType { Id = 1, Name = "Type1", Threshold = 5 };
            tester = new Tester { Id = 1, Name = "Tester1", Pc = new Pc { Id = 1, Name = "PC1" } };
            probe = new Probe { Id = 1, Sn = "ProbeSN", MotorModuleId = 1, TransducerModuleId = 1 };
            transducerModule = new TransducerModule { Id = 1, Sn = "TransducerModuleSN" };
            transducer = new Transducer { Id = 1, Sn = "TransducerSN" };
            motorModule = new MotorModule { Id = 1, Sn = "MotorModuleSN" };
            //InitializeTestData();
        }

        private void InitializeTestData()
        {
            testCategory = new TestCategory { Id = 1, Name = "Category1" };
            testType = new TestType { Id = 1, Name = "Type1", Threshold = 5 };
            tester = new Tester { Id = 1, Name = "Tester1", Pc = new Pc { Id = 1, Name = "PC1" } };
            probe = new Probe { Id = 1, Sn = "ProbeSN", MotorModuleId = 1, TransducerModuleId = 1 };
            transducerModule = new TransducerModule { Id = 1, Sn = "TransducerModuleSN" };
            transducer = new Transducer { Id = 1, Sn = "TransducerSN" };
            motorModule = new MotorModule { Id = 1, Sn = "MotorModuleSN" };
        }

        [Fact]
        public void Convert_ShouldReturnNull_WhenTestIsNull()
        {
            // Arrange
            Test test = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => TestToTestProbe.Convert(test));
        }

        [Fact]
        public void Convert_ShouldReturnTestProbe_WhenTestIsValid()
        {
            // Arrange
            var test = CreateTest();

            // Act
            var result = TestToTestProbe.Convert(test);

            // Assert
            AssertTestProbe(result, test);
        }

        [Fact]
        public void ToList_ShouldReturnEmptyList_WhenTestsIsNull()
        {
            // Arrange
            IEnumerable<Test> tests = null;

            Assert.Throws<ArgumentNullException>(() => TestToTestProbe.ToList(tests));
        }

        [Fact]
        public void ToList_ShouldReturnTestProbeList_WhenTestsIsValid()
        {
            // Arrange
            var tests = new List<Test> { CreateTest(1), CreateTest(2) };

            // Act
            var result = TestToTestProbe.ToList(tests);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            AssertTestProbe(result.ElementAt(0), tests[0]);
            AssertTestProbe(result.ElementAt(1), tests[1]);
        }

        private Test CreateTest(int id = 1)
        {
            return new Test
            {
                Id = id,
                CreatedDate = DateTime.Now,
                Detail = $"Test Detail {id}",
                TestCategory = testCategory,
                TestType = testType,
                Tester = tester,
                OriginalImg = "OriginalImg",
                ChangedImg = "ChangedImg",
                ChangedImgMetadata = "Metadata",
                Result = 100,
                Method = 1,
                Probe = probe,
                TransducerModule = transducerModule,
                Transducer = transducer
            };
        }

        private void AssertTestProbe(TestProbe result, Test test)
        {
            Assert.NotNull(result);
            Assert.Equal(test.Id, result.Id);
            Assert.Equal(test.CreatedDate, result.CreatedDate);
            Assert.Equal(test.Detail, result.Detail);
            Assert.Equal(test.TestCategory, result.Category);
            Assert.Equal(test.TestType, result.TestType);
            Assert.Equal(test.Tester.Name, result.Tester);
            Assert.Equal(test.Tester.Pc.Name, result.Pc.Name);
            Assert.Equal(test.OriginalImg, result.OriginalImg);
            Assert.Equal(test.ChangedImg, result.ChangedImg);
            Assert.Equal(test.ChangedImgMetadata, result.ChangedImgMetadata);
            Assert.Equal(test.Result, result.Result);
            Assert.Equal(test.Method, result.Method);
            Assert.Equal(test.Probe, result.Probe);
            Assert.Equal(test.TransducerModule, result.TransducerModule);
            Assert.Equal(test.Transducer, result.Transducer);
            Assert.Equal(test.Probe.MotorModule, result.MotorModule);
        }
    }
}
