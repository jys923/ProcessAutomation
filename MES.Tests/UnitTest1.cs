using MES.UI.Models;
using MES.UI.Models.Context;
using MES.UI.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MES.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task SetGetAll()
        {
            // Arrange
            var items = new List<TestType>
            {
                new TestType { Name = "1"},
                new TestType { Name = "2"},
                new TestType { Name = "3"},
                new TestType { Name = "4"},

            };

            Mock<MESDbContext> dbContextMock = new Mock<MESDbContext>();
            Mock<DbSet<TestType>> dbSetMock = new Mock<DbSet<TestType>>();

            dbSetMock.As<IQueryable<TestType>>().Setup(m => m.Provider).Returns(items.AsQueryable().Provider);
            dbSetMock.As<IQueryable<TestType>>().Setup(m => m.Expression).Returns(items.AsQueryable().Expression);
            dbSetMock.As<IQueryable<TestType>>().Setup(m => m.ElementType).Returns(items.AsQueryable().ElementType);
            dbSetMock.As<IQueryable<TestType>>().Setup(m => m.GetEnumerator()).Returns(items.GetEnumerator());

            dbContextMock.Setup(db => db.TestTypes).Returns(dbSetMock.Object);

            TestTypeRepository dataService = new TestTypeRepository(dbContextMock.Object);

            Task<bool> res = dataService.InsertAsync(new TestType { Name = "1" });

            // Act
            IEnumerable<TestType> result = await dataService.GetAllAsync();

            // Assert
            Assert.Equal(3, result.ToList().Count);
            Assert.Contains(result, i => i.Id == 1 && i.Name == "1");
            Assert.Contains(result, i => i.Id == 2 && i.Name == "2");
            Assert.Contains(result, i => i.Id == 3 && i.Name == "3");
        }
    }
}