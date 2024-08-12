//using Moq;
//using SonoCap.MES.Models;
//using SonoCap.MES.Repositories;
//using SonoCap.MES.Repositories.Context;
//using Microsoft.EntityFrameworkCore;

//namespace SonoCap.Tests.MES.Repositories
//{
//    public class TestRepositoryTests
//    {
//        private readonly Mock<DbSet<Test>> _mockTestSet;
//        private readonly Mock<DbSet<Probe>> _mockProbeSet;
//        private readonly Mock<DbSet<TransducerModule>> _mockTransducerModuleSet;
//        private readonly Mock<DbSet<Transducer>> _mockTransducerSet;
//        private readonly Mock<DbSet<MotorModule>> _mockMotorModuleSet;
//        private readonly Mock<MESDbContext> _mockContext;
//        private readonly TestRepository _repository;

//        public TestRepositoryTests()
//        {
//            // Mock DbSets
//            _mockTestSet = new Mock<DbSet<Test>>();
//            _mockProbeSet = new Mock<DbSet<Probe>>();
//            _mockTransducerModuleSet = new Mock<DbSet<TransducerModule>>();
//            _mockTransducerSet = new Mock<DbSet<Transducer>>();
//            _mockMotorModuleSet = new Mock<DbSet<MotorModule>>();

//            // Mock DbContext
//            _mockContext = new Mock<MESDbContext>();
//            _mockContext.Setup(c => c.Set<Test>()).Returns(_mockTestSet.Object);
//            _mockContext.Setup(c => c.Set<Probe>()).Returns(_mockProbeSet.Object);
//            _mockContext.Setup(c => c.Set<TransducerModule>()).Returns(_mockTransducerModuleSet.Object);
//            _mockContext.Setup(c => c.Set<Transducer>()).Returns(_mockTransducerSet.Object);
//            _mockContext.Setup(c => c.Set<MotorModule>()).Returns(_mockMotorModuleSet.Object);

//            // Initialize repository
//            Mock<IDbContextFactory<MESDbContext>> contextFactory = new Mock<IDbContextFactory<MESDbContext>>();
//            contextFactory.Setup(cf => cf.CreateDbContext()).Returns(_mockContext.Object);
//            _repository = new TestRepository(contextFactory.Object);

//            // Seed data
//            SetupMockData();
//        }

//        private void SetupMockData()
//        {
//            var tests = new List<Test>
//            {
//                new Test
//                {
//                    Id = 1,
//                    CreatedDate = System.DateTime.Now,
//                    DataFlag = 1,
//                    ProbeId = 1,
//                    TransducerModuleId = 1,
//                    TransducerId = 1,
//                },
//                new Test
//                {
//                    Id = 2,
//                    CreatedDate = System.DateTime.Now.AddDays(-1),
//                    DataFlag = 1,
//                    ProbeId = 2,
//                    TransducerModuleId = 2,
//                    TransducerId = 2,
//                }
//            }.AsQueryable();

//            var probes = new List<Probe>
//            {
//                new Probe { Id = 1, Sn = "P123", TransducerModuleId = 1, MotorModuleId = 1 },
//                new Probe { Id = 2, Sn = "P124", TransducerModuleId = 2, MotorModuleId = 2 }
//            }.AsQueryable();

//            var transducerModules = new List<TransducerModule>
//            {
//                new TransducerModule { Id = 1, Sn = "TM123" },
//                new TransducerModule { Id = 2, Sn = "TM124" }
//            }.AsQueryable();

//            var transducers = new List<Transducer>
//            {
//                new Transducer { Id = 1, Sn = "T123" },
//                new Transducer { Id = 2, Sn = "T124" }
//            }.AsQueryable();

//            var motorModules = new List<MotorModule>
//            {
//                new MotorModule { Id = 1, Sn = "M123" },
//                new MotorModule { Id = 2, Sn = "M124" }
//            }.AsQueryable();

//            // Setup DbSet mock behaviors
//            _mockTestSet.As<IQueryable<Test>>().Setup(m => m.Provider).Returns(tests.Provider);
//            _mockTestSet.As<IQueryable<Test>>().Setup(m => m.Expression).Returns(tests.Expression);
//            _mockTestSet.As<IQueryable<Test>>().Setup(m => m.ElementType).Returns(tests.ElementType);
//            _mockTestSet.As<IQueryable<Test>>().Setup(m => m.GetEnumerator()).Returns(tests.GetEnumerator());

//            _mockProbeSet.As<IQueryable<Probe>>().Setup(m => m.Provider).Returns(probes.Provider);
//            _mockProbeSet.As<IQueryable<Probe>>().Setup(m => m.Expression).Returns(probes.Expression);
//            _mockProbeSet.As<IQueryable<Probe>>().Setup(m => m.ElementType).Returns(probes.ElementType);
//            _mockProbeSet.As<IQueryable<Probe>>().Setup(m => m.GetEnumerator()).Returns(probes.GetEnumerator());

//            _mockTransducerModuleSet.As<IQueryable<TransducerModule>>().Setup(m => m.Provider).Returns(transducerModules.Provider);
//            _mockTransducerModuleSet.As<IQueryable<TransducerModule>>().Setup(m => m.Expression).Returns(transducerModules.Expression);
//            _mockTransducerModuleSet.As<IQueryable<TransducerModule>>().Setup(m => m.ElementType).Returns(transducerModules.ElementType);
//            _mockTransducerModuleSet.As<IQueryable<TransducerModule>>().Setup(m => m.GetEnumerator()).Returns(transducerModules.GetEnumerator());

//            _mockTransducerSet.As<IQueryable<Transducer>>().Setup(m => m.Provider).Returns(transducers.Provider);
//            _mockTransducerSet.As<IQueryable<Transducer>>().Setup(m => m.Expression).Returns(transducers.Expression);
//            _mockTransducerSet.As<IQueryable<Transducer>>().Setup(m => m.ElementType).Returns(transducers.ElementType);
//            _mockTransducerSet.As<IQueryable<Transducer>>().Setup(m => m.GetEnumerator()).Returns(transducers.GetEnumerator());

//            _mockMotorModuleSet.As<IQueryable<MotorModule>>().Setup(m => m.Provider).Returns(motorModules.Provider);
//            _mockMotorModuleSet.As<IQueryable<MotorModule>>().Setup(m => m.Expression).Returns(motorModules.Expression);
//            _mockMotorModuleSet.As<IQueryable<MotorModule>>().Setup(m => m.ElementType).Returns(motorModules.ElementType);
//            _mockMotorModuleSet.As<IQueryable<MotorModule>>().Setup(m => m.GetEnumerator()).Returns(motorModules.GetEnumerator());
//        }

//        [Fact]
//        public async Task GetTestAsync_ShouldReturnFilteredTests()
//        {
//            // Arrange
//            var startDate = System.DateTime.Now.AddDays(-2);
//            var endDate = System.DateTime.Now;

//            // Act
//            var result = await _repository.GetTestAsync(
//                startDate: startDate,
//                endDate: endDate,
//                categoryId: null,
//                testTypeId: null,
//                tester: null,
//                pcId: null,
//                result: null,
//                dataFlagTest: null,
//                probeSn: "P123",
//                transducerModuleSn: null,
//                transducerSn: null,
//                motorModuleSn: null,
//                dataFlagProbe: null
//            );

//            // Assert
//            Assert.Single(result);
//            Assert.Equal(1, result.First().Id);
//        }

//        [Fact]
//        public async Task GetTestProbeLinqAsync2_ShouldReturnTestProbeDetails()
//        {
//            // Arrange
//            var startDate = System.DateTime.Now.AddDays(-2);
//            var endDate = System.DateTime.Now;

//            // Act
//            var result = await _repository.GetTestProbeLinqAsync2(
//                startDate: startDate,
//                endDate: endDate,
//                categoryId: null,
//                testTypeId: null,
//                tester: null,
//                pcId: null,
//                result: null,
//                dataFlagTest: null,
//                probeSn: "P123",
//                transducerModuleSn: null,
//                transducerSn: null,
//                motorModuleSn: null,
//                dataFlagProbe: null
//            );

//            // Assert
//            Assert.Single(result);
//            var testProbe = result.First();
//            Assert.Equal(1, testProbe.Id);
//            Assert.Equal("P123", testProbe.Probe.Sn);
//            Assert.Equal("TM123", testProbe.TransducerModule.Sn);
//            Assert.Equal("T123", testProbe.Transducer.Sn);
//            Assert.Equal("M123", testProbe.MotorModule.Sn);
//        }
//    }
//}
