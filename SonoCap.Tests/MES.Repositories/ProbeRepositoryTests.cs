using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SonoCap.MES.Repositories.Context;
using SonoCap.Tests.MES.Repositories.Base;
using SQLitePCL;
using SonoCap.MES.Repositories.Interfaces;

namespace SonoCap.MES.Repositories.Tests
{
    public class ProbeRepositoryTests : IDisposable
    {
        private readonly IProbeRepository _repository;
        private readonly MESDbContext _context;

        public ProbeRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<MESDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new MESDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new ProbeRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        //[Fact]
        //public async Task SetPTRViewsAsync_ShouldInsertRecords()
        //{
        //    TestHelper.SeedDatabaseForDetailedTests(_context);
        //    TestHelper.SeedDatabaseForDetailedTestsProbe(_context);

        //    var result = await _repository.SetPTRViewsAsync();

        //    Assert.Equal(1, result); // 쿼리에 의해 1개의 행이 삽입되었는지 확인

        //    var insertedRecord = await _context.PTRViews.FirstOrDefaultAsync();
        //    Assert.NotNull(insertedRecord);
        //    Assert.Equal("ProbeSN1", insertedRecord?.ProbeSn);
        //    Assert.Equal("TransducerModuleSN1", insertedRecord?.TransducerModuleSn);
        //    Assert.Equal("TransducerSN1", insertedRecord?.TransducerSn);
        //    Assert.Equal("MotorModuleSN1", insertedRecord?.MotorModuleSn);
        //    Assert.Equal(1, insertedRecord?.DataFlag);
        //}

        [Fact]
        public async Task GetPTRViewAsync_ShouldReturnPTRView()
        {
            TestHelper.SeedDatabaseForDetailedTests(_context);
            TestHelper.SeedDatabaseForDetailedTestsProbe(_context);

            //await _repository.SetPTRViewsAsync();
            var result = await _repository.GetPTRViewAsync("ProbeSN1");

            Assert.NotNull(result);
            Assert.Equal("ProbeSN1", result?.ProbeSn);
            Assert.Equal("TransducerModuleSN1", result?.TransducerModuleSn);
            Assert.Equal("TransducerSN1", result?.TransducerSn);
            Assert.Equal("MotorModuleSN1", result?.MotorModuleSn);
            Assert.Equal(1, result?.TestId01);
        }
    }
}
