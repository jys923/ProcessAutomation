using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories;
using SonoCap.MES.Repositories.Context;

namespace SonoCap.MES.Repositories.Tests
{
    public class SharedSeqNoRepositoryTests : IDisposable
    {
        private readonly SharedSeqNoRepository _repository;
        private readonly MESDbContext _context;

        public SharedSeqNoRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<MESDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDb")
                .Options;

            _context = new MESDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new SharedSeqNoRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task InitializeAsync_ShouldInsertSeqNoIfNotExists()
        {
            // Act
            await _repository.InitializeAsync();

            // Assert
            var seqNo = await _repository.GetSeqNoAsync();
            Assert.NotNull(seqNo);
            Assert.Equal(DateTime.Today, seqNo?.Date);
        }

        [Fact]
        public async Task GetSeqNoAsync_ShouldReturnSeqNoIfExists()
        {
            // Arrange
            var date = DateTime.Today;
            _context.SharedSeqNos.Add(new SharedSeqNo { Date = date });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetSeqNoAsync(date);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(date, result?.Date);
        }

        [Fact]
        public async Task SetSeqNoAsync_ShouldIncrementSeqNo()
        {
            // Arrange
            var date = DateTime.Today;
            _context.SharedSeqNos.Add(new SharedSeqNo { Date = date, TDMdNo = 1, ProbeNo = 1 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SetSeqNoAsync(date);

            // Assert
            Assert.True(result);

            var seqNo = await _repository.GetSeqNoAsync(date);
            Assert.NotNull(seqNo);
            Assert.Equal(2, seqNo?.TDMdNo);
            Assert.Equal(2, seqNo?.ProbeNo);
        }

        [Fact]
        public async Task SetSeqNoAsync_WithType_ShouldIncrementSpecificSeqNo()
        {
            // Arrange
            var date = DateTime.Today;
            _context.SharedSeqNos.Add(new SharedSeqNo { Date = date, TDMdNo = 1, ProbeNo = 1 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SetSeqNoAsync(SnType.TransducerModule, date);

            // Assert
            Assert.True(result);

            var seqNo = await _repository.GetSeqNoAsync(date);
            Assert.NotNull(seqNo);
            Assert.Equal(2, seqNo?.TDMdNo);
            Assert.Equal(1, seqNo?.ProbeNo); // Only TDMdNo should be incremented
        }

        [Fact]
        public async Task UpsertSeqNoAsync_ShouldInsertOrUpdateSeqNo()
        {
            // Arrange
            var date = DateTime.Today;

            // Act
            var seqNo = await _repository.UpsertSeqNoAsync(SnType.TransducerModule, date);

            // Assert
            Assert.NotNull(seqNo);
            Assert.Equal(date, seqNo?.Date);
            Assert.Equal(1, seqNo?.TDMdNo);
            Assert.Equal(1, seqNo?.ProbeNo);

            // Act - Update
            seqNo = await _repository.UpsertSeqNoAsync(SnType.TransducerModule, date);

            // Assert
            Assert.NotNull(seqNo);
            Assert.Equal(date, seqNo?.Date);
            Assert.Equal(2, seqNo?.TDMdNo);
            Assert.Equal(1, seqNo?.ProbeNo);
        }
    }

}
