using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories;
using SonoCap.MES.Repositories.Context;
using Xunit;

namespace SonoCap.MES.Repositories.Tests
{
    public class PcRepositoryTests
    {
        private readonly Mock<DbSet<Pc>> _mockSet;
        private readonly Mock<MESDbContext> _mockContext;
        private readonly PcRepository _repository;

        public PcRepositoryTests()
        {
            _mockSet = new Mock<DbSet<Pc>>();
            _mockContext = new Mock<MESDbContext>();

            _mockContext.Setup(m => m.Set<Pc>()).Returns(_mockSet.Object);

            _repository = new PcRepository(_mockContext.Object);
        }

        [Fact]
        public async Task InsertAsync_ShouldAddEntity()
        {
            var pc = new Pc { Name = "Test PC" };

            _mockSet.Setup(m => m.Add(pc)).Returns((EntityEntry<Pc>)null!);
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _repository.InsertAsync(pc);

            _mockSet.Verify(m => m.Add(It.IsAny<Pc>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntity()
        {
            var pc = new Pc { Name = "Test PC" };

            _mockSet.Setup(m => m.Remove(It.IsAny<Pc>()));
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            await _repository.InsertAsync(pc);
            var result = await _repository.DeleteAsync(pc);

            Assert.True(result);
            _mockSet.Verify(m => m.Remove(It.IsAny<Pc>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity()
        {
            var pc = new Pc { Name = "Test PC" };

            _mockSet.Setup(m => m.FindAsync(It.IsAny<int>())).ReturnsAsync(pc);

            var result = await _repository.GetByIdAsync(pc.Id);

            Assert.NotNull(result);
            Assert.Equal(pc.Id, result?.Id);
        }
    }
}
