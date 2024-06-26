using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using SonoCap.MES.Repositories.Interfaces;

namespace SonoCap.MES.Repositories
{
    public class TestRepository : RepositoryBase<Test>, ITestRepository
    {
        public TestRepository(MESDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public async Task<List<TestProbe>> GetTestProbeLinqAsync(
            DateTime? startDate,
            DateTime? endDate,
            int? categoryId,
            int? testTypeId,
            string? tester,
            int? pcId,
            int? result,
            int? dataFlagTest,
            string? probeSn,
            string? transducerModuleSn,
            string? transducerSn,
            string? motorModuleSn,
            int? dataFlagProbe)
        {
            IQueryable<TestProbe> query =
                (from t in _context.Set<Test>()
                 join p in _context.Set<Probe>() on t.ProbeId equals p.Id into pGroup
                 from p in pGroup.DefaultIfEmpty()
                 join tm in _context.Set<TransducerModule>() on t.TransducerModuleId equals tm.Id into tmGroup
                 from tm in tmGroup.DefaultIfEmpty()
                 join td in _context.Set<Transducer>() on t.TransducerId equals td.Id into tdGroup
                 from td in tdGroup.DefaultIfEmpty()
                 join mm in _context.Set<MotorModule>() on p.MotorModuleId equals mm.Id into mmGroup
                 from mm in mmGroup.DefaultIfEmpty()
                 where t.DataFlag == 1
                    && t.Id < 100000
                    && (startDate == null || t.CreatedDate >= startDate)
                    && (endDate == null || t.CreatedDate <= endDate)
                    && (string.IsNullOrEmpty(probeSn) || p.Sn.Contains(probeSn))
                    && (string.IsNullOrEmpty(transducerModuleSn) || tm.Sn.Contains(transducerModuleSn))
                    && (string.IsNullOrEmpty(transducerSn) || td.Sn.Contains(transducerSn))
                    && (string.IsNullOrEmpty(motorModuleSn) || mm.Sn.Contains(motorModuleSn))
                 select new TestProbe
                 {
                     Id = t.Id,
                     CreatedDate = t.CreatedDate,
                     Detail = t.Detail,
                     Category = t.TestCategory,
                     TestType = t.TestType,
                     Tester = t.Tester.Name,
                     Pc = t.Tester.Pc,
                     OriginalImg = t.OriginalImg,
                     ChangedImg = t.ChangedImg,
                     ChangedImgMetadata = t.ChangedImgMetadata,
                     Result = t.Result,
                     Method = t.Method,
                     Probe = p,
                     TransducerModule = tm,
                     Transducer = td,
                     MotorModule = mm,
                 });

            return await query.ToListAsync();
        }

    }
}
