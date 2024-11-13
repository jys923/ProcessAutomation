using Microsoft.EntityFrameworkCore;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.Repositories;

namespace SonoCap.MES.Repositories.Tests
{
    public class MotorModuleRepositoryTests
    {
        private readonly IMotorModuleRepository _repository;
        private readonly MESDbContext _context;

        public MotorModuleRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<MESDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new MESDbContext(options);
            _repository = new MotorModuleRepository(_context);
        }

        [Fact]
        public async Task InsertAsync_ShouldAddEntity()
        {
            var motorModule = new MotorModule { Sn = "module-sn1" };
            var result = await _repository.InsertAsync(motorModule);

            Assert.True(result);
            Assert.NotEqual(0, motorModule.Id);
        }

        //[Fact]
        //public async Task GetAllAsync_ShouldReturnAllEntities()
        //{
        //    var motorModule1 = new MotorModule { Sn = "module-sn2" };
        //    var motorModule2 = new MotorModule { Sn = "module-sn3" };

        //    await _repository.InsertAsync(motorModule1);
        //    await _repository.InsertAsync(motorModule2);

        //    var result = await _repository.GetAllAsync();

        //    Assert.Equal(2, result.Count());
        //}

        //[Fact]
        public async Task BulkInsertAsync_ShouldAddEntities()
        {
            List<MotorModule> entities = new List<MotorModule>();

            entities.Add(new MotorModule { Sn = "module-sn4" });
            entities.Add(new MotorModule { Sn = "module-sn5" });

            var result = await _repository.BulkInsertAsync(entities);

            Assert.True(result);
            var fromDb = await _repository.GetAllAsync();
            Assert.Equal(2, fromDb.Count());
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntity()
        {
            var motorModule = new MotorModule { Sn = "module-sn6" };

            await _repository.InsertAsync(motorModule);
            var deleteResult = await _repository.DeleteAsync(motorModule);

            Assert.True(deleteResult);
            var fromDb = await _repository.GetByIdAsync(motorModule.Id);
            Assert.Null(fromDb);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity()
        {
            var motorModule = new MotorModule { Sn = "module-sn7" };

            await _repository.InsertAsync(motorModule);
            var fromDb = await _repository.GetByIdAsync(motorModule.Id);

            Assert.NotNull(fromDb);
            Assert.Equal(motorModule.Sn, fromDb.Sn);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldRemoveEntity()
        {
            var motorModule = new MotorModule { Sn = "module-sn8" };

            await _repository.InsertAsync(motorModule);
            var deleteResult = await _repository.DeleteByIdAsync(motorModule.Id);

            Assert.True(deleteResult);
            var fromDb = await _repository.GetByIdAsync(motorModule.Id);
            Assert.Null(fromDb);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyEntity()
        {
            var motorModule = new MotorModule { Sn = "module-sn9" };

            await _repository.InsertAsync(motorModule);
            motorModule.Sn = "module-sn456";
            var updateResult = await _repository.UpdateAsync(motorModule);

            Assert.True(updateResult);
            var fromDb = await _repository.GetByIdAsync(motorModule.Id);
            Assert.NotNull(fromDb);
            Assert.Equal("module-sn456", fromDb.Sn);
        }

        [Fact]
        public void GetQueryable_ShouldReturnQueryable()
        {
            var queryable = _repository.GetQueryable();

            Assert.NotNull(queryable);
            Assert.IsAssignableFrom<IQueryable<MotorModule>>(queryable);
        }

        [Fact]
        public async Task GetBySn_ShouldReturnEntities()
        {
            var motorModule = new MotorModule { Sn = "module-sn10" };

            await _repository.InsertAsync(motorModule);
            var result = _repository.GetBySn("module-sn10");

            Assert.Single(result);
            Assert.Equal("module-sn10", result.First().Sn);
        }

        //[Fact]
        //public async Task UpsertAsync_ShouldInsertOrUpdateEntity()
        //{
        //    var motorModule = new MotorModule { Sn = "module-sn11" };

        //    var insertResult = await _repository.UpsertAsync(motorModule);
        //    //Assert.Equal(1, insertResult);

        //    motorModule.Id = insertResult;
        //    motorModule.Sn = "module-sn12";
        //    var updateResult = await _repository.UpsertAsync(motorModule);
        //    Assert.Equal(insertResult, updateResult);

        //    var fromDb = await _repository.GetByIdAsync(motorModule.Id);
        //    Assert.NotNull(fromDb);
        //    Assert.Equal("module-sn12", fromDb.Sn);
        //}

        //[Fact]
        public void GetFilterItems_ShouldReturnFilteredItems()
        {
            var entities = new List<MotorModule>
            {
                new MotorModule { Sn = "module-sn1234" },
                new MotorModule { Sn = "module-sn12344" }
            };

            _repository.BulkInsertAsync(entities).Wait();

            var result = _repository.GetFilterItems("module-sn1234", 1);

            Assert.Single(result);
            Assert.Contains(result, e => e.Sn == "module-sn123");
        }
    }
}
