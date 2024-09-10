using Microsoft.EntityFrameworkCore;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Context;
using System.ComponentModel.DataAnnotations;

namespace SonoCap.MES.Models.Tests
{
    public class PcTests
    {
        private readonly DbContextOptions<MESDbContext> _options;

        public PcTests()
        {
            _options = new DbContextOptionsBuilder<MESDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryPcTestDb")
                .Options;

            using var context = new MESDbContext(_options);
            context.Database.EnsureCreated();
        }

        [Fact]
        public async Task AddPc_ShouldSavePc()
        {
            // Arrange
            var pc = new Pc { Name = "PC1" };

            // Act
            using (var context = new MESDbContext(_options))
            {
                context.Pcs.Add(pc);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new MESDbContext(_options))
            {
                var result = await context.Pcs.FirstOrDefaultAsync(p => p.Name == "PC1");
                Assert.NotNull(result);
                Assert.Equal("PC1", result?.Name);
            }
        }

        [Fact]
        public async Task AddPc_WithoutName_ShouldThrowDbUpdateException2()
        {
            // Arrange
            var pc = new Pc { Name = null! }; // Name 필드가 없는 경우

            // Act & Assert
            using (var context = new MESDbContext(_options))
            {
                context.Pcs.Add(pc);
                await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
            }
        }


        [Fact]
        public void AddPc_WithoutName_ShouldThrowValidationException()
        {
            // Arrange
            var pc = new Pc { Name = null! }; // Name 필드가 없는 경우

            // Act & Assert
            using (var context = new MESDbContext(_options))
            {
                context.Pcs.Add(pc);

                // 유효성 검사 수행
                var validationContext = new ValidationContext(pc);
                var validationResults = new List<ValidationResult>();

                bool isValid = Validator.TryValidateObject(pc, validationContext, validationResults, true);

                Assert.False(isValid);
                Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(pc.Name)));
            }
        }

    }
}