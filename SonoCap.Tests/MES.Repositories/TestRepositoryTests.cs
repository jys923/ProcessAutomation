using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories;
using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.Tests.MES.Repositories.Base;

namespace SonoCap.MES.Repositories.Tests
{
    public class TestRepositoryTests : IDisposable
    {
        private readonly ITestRepository _repository;
        private readonly MESDbContext _context;

        public TestRepositoryTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<MESDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));
            var serviceProvider = serviceCollection.BuildServiceProvider();

            _context = serviceProvider.GetRequiredService<MESDbContext>();
            _repository = new TestRepository(_context);

            TestHelper.SeedDatabase(_context);
        }

        //[Fact]
        public async Task GetTestAsync_ReturnsFilteredResults()
        {
            var tests = _context.Tests.ToList();  // 모든 Test 엔터티 조회
            foreach (var test1 in tests)
            {
                Console.WriteLine($"Id: {test1.Id}, CreatedDate: {test1.CreatedDate}");
            }
            //var test = new Test
            //{
            //    CreatedDate = DateTime.Now,
            //    DataFlag = 1,
            //    TestCategoryId = testCategory1.Id,
            //    TestTypeId = testType1.Id,
            //    TesterId = tester1.Id,
            //    ProbeId = probe.Id,
            //    TransducerModuleId = transducerModule.Id,
            //    TransducerId = transducer.Id,
            //    Result = 10
            //};
            // Act
            var result = await _repository.GetTestAsync(
                startDate: DateTime.Now.AddDays(-1),
                endDate: DateTime.Now,
                categoryId: 1,
                testTypeId: 1,
                tester: "Tester1",
                pcId: 1,
                result: 10,
                dataFlagTest: 1,
                probeSn: "ProbeSN",
                transducerModuleSn: "TransducerModuleSN",
                transducerSn: "TransducerSN",
                motorModuleSn: "MotorModuleSN",
                dataFlagProbe: 1);

            // Assert
            Assert.Single(result);
            var test = result.First();
            Assert.Equal(1, test.Id);
        }

        //[Fact]
        public void GetLatestTests_ThrowsExceptionWhenNoParameters()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _repository.GetLatestTests());
            Assert.Equal("At least one of 'transducer', 'transducerModule', or 'probe' must be provided.", exception.Message);
        }

        //[Fact]
        public void GetLatestTests_ReturnsLatestTests()
        {
            // Arrange
            var transducer = new Transducer { Id = 1, Sn = "TransducerSn1" };

            // Act
            var result = _repository.GetLatestTests(transducer: transducer);

            // Assert
            Assert.Single(result);
            var test = result.First();
            Assert.Equal(1, test.Id);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
