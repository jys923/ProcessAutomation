using SonoCap.MES.Models;
using Serilog;

namespace SonoCap.MES.Repositories.Context
{
    public static class DbContextExtensions
    {
        public static async Task SeedAsync(this MESDbContext context)
        {
            // 중복 방지용 체크
            if (!context.Pcs.Any())
            {
                await context.Pcs.AddRangeAsync(
                    new Pc { Name = "left" },
                    new Pc { Name = "middle" },
                    new Pc { Name = "right" }
                );
            }

            if (!context.TestCategories.Any())
            {
                await context.TestCategories.AddRangeAsync(
                    new TestCategory { Name = "공정용" },
                    new TestCategory { Name = "최종용" },
                    new TestCategory { Name = "출하용" }
                );
            }

            if (!context.TestTypes.Any())
            {
                await context.TestTypes.AddRangeAsync(
                    new TestType { Name = "Gray" },
                    new TestType { Name = "Res" },
                    new TestType { Name = "Align" }
                );
            }

            if (!context.TransducerTypes.Any())
            {
                await context.TransducerTypes.AddRangeAsync(
                    new TransducerType { Code = "G1", Type = "5Mhz" },
                    new TransducerType { Code = "G2", Type = "7.5Mhz" }
                );
            }

            await context.SaveChangesAsync();
            Log.Information("Seed 데이터 자동 삽입 완료");
        }
    }
}
