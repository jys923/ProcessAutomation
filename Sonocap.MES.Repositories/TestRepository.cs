using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using SonoCap.MES.Repositories.Interfaces;

namespace SonoCap.MES.Repositories
{
    public class TestRepository : RepositoryBase<Test>, ITestRepository
    {
        public TestRepository(MESDbContext context) : base(context)
        {
        }

        public List<TestProbe> GetTestProbe2()
        {
            IEnumerable<TestProbe> probes =
                (from t in _context.Set<Test>()
                 join p in _context.Set<Probe>() on t.TransducerModule equals p.TransducerModule into pGroup
                 from p in pGroup.DefaultIfEmpty()
                 select new TestProbe
                 {
                     Id = t.Id,
                     CreatedDate = t.CreatedDate,
                     Detail = t.Detail,
                     Category = t.Category,
                     TestType = t.TestType,
                     Tester = t.Tester.Name,
                     Pc = t.Tester.Pc,
                     OriginalImg = t.OriginalImg,
                     ChangedImg = t.ChangedImg,
                     ChangedImgMetadata = t.ChangedImgMetadata,
                     Result = t.Result,
                     Method = t.Method,
                     TransducerModule = t.TransducerModule,
                     ProbeSn = p.ProbeSn,
                     MotorModule = p.MotorModule
                 });
            return probes.ToList();
        }

        public List<TestProbe> GetTestProbe()
        {
            List<TestProbe> testprobes =
                (from t in _context.Set<Test>()
                 join p in _context.Set<Probe>() on t.TransducerModuleId equals p.TransducerModuleId into pGroup
                 from p in pGroup.DefaultIfEmpty()
                 select new TestProbe
                 {
                     Id = t.Id,
                     CreatedDate = t.CreatedDate,
                     Detail = t.Detail,
                     Category = t.Category,
                     TestType = t.TestType,
                     Tester = t.Tester.Name,
                     Pc = t.Tester.Pc,
                     OriginalImg = t.OriginalImg,
                     ChangedImg = t.ChangedImg,
                     ChangedImgMetadata = t.ChangedImgMetadata,
                     Result = t.Result,
                     Method = t.Method,
                     TransducerModule = t.TransducerModule,
                     ProbeSn = p.ProbeSn,
                     MotorModule = p.MotorModule
                 }).ToList();

            return testprobes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<TestProbe>> GetTestProbeAsync()
        {
            IQueryable<TestProbe> testprobes =
                (from t in _context.Set<Test>()
                 join p in _context.Set<Probe>() on t.TransducerModuleId equals p.TransducerModuleId into pGroup
                 from p in pGroup.DefaultIfEmpty()
                 select new TestProbe
                 {
                     Id = t.Id,
                     CreatedDate = t.CreatedDate,
                     Detail = t.Detail,
                     Category = t.Category,
                     TestType = t.TestType,
                     Tester = t.Tester.Name,
                     Pc = t.Tester.Pc,
                     OriginalImg = t.OriginalImg,
                     ChangedImg = t.ChangedImg,
                     ChangedImgMetadata = t.ChangedImgMetadata,
                     Result = t.Result,
                     Method = t.Method,
                     TransducerModule = t.TransducerModule,
                     ProbeSn = p.ProbeSn,
                     MotorModule = p.MotorModule
                 });

            return await testprobes.ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="categoryId"></param>
        /// <param name="testTypeId"></param>
        /// <param name="tester"></param>
        /// <param name="pcId"></param>
        /// <param name="result"></param>
        /// <param name="probeSn"></param>
        /// <param name="transducerModuleSn"></param>
        /// <param name="transducerSn"></param>
        /// <param name="motorModuleSn"></param>
        /// <returns></returns>
        public List<TestProbe> GetTestProbe(DateTime? startDate, DateTime? endDate, int? categoryId, int? testTypeId, string? tester, int? pcId, int? result, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            IQueryable<TestProbe> query =
                (from t in _context.Set<Test>()
                 join p in _context.Set<Probe>() on t.TransducerModuleId equals p.TransducerModuleId into pGroup
                 from p in pGroup.DefaultIfEmpty()
                 select new TestProbe
                 {
                     Id = t.Id,
                     CreatedDate = t.CreatedDate,
                     Detail = t.Detail,
                     Category = t.Category,
                     TestType = t.TestType,
                     Tester = t.Tester.Name,
                     Pc = t.Tester.Pc,
                     OriginalImg = t.OriginalImg,
                     ChangedImg = t.ChangedImg,
                     ChangedImgMetadata = t.ChangedImgMetadata,
                     Result = t.Result,
                     Method = t.Method,
                     TransducerModule = t.TransducerModule,
                     ProbeSn = p.ProbeSn,
                     MotorModule = p.MotorModule
                 });

            if (startDate != null)
            {
                query = query.Where(tp => tp.CreatedDate >= startDate);
            }

            if (endDate != null)
            {
                query = query.Where(tp => tp.CreatedDate <= endDate);
            }

            if (categoryId != null && categoryId != 0)
            {
                query = query.Where(tp => tp.Category.Id == categoryId);
            }

            if (testTypeId != null && testTypeId != 0)
            {
                query = query.Where(tp => tp.TestType.Id == testTypeId);
            }

            if (!string.IsNullOrEmpty(tester))
            {
                query = query.Where(tp => tp.Tester == tester);
            }

            if (pcId != null && pcId != 0)
            {
                query = query.Where(tp => tp.Pc.Id == pcId);
            }

            if (result != null && result != 0)
            {
                if (result == 1)
                {
                    query = query.Where(tp => tp.Result > 2);
                }
                else if (result == 2)
                {
                    query = query.Where(tp => tp.Result == 2);
                }
            }

            if (!string.IsNullOrEmpty(probeSn))
            {
                query = query.Where(tp => tp.ProbeSn!.Contains(probeSn));
            }

            if (!string.IsNullOrEmpty(transducerModuleSn))
            {
                query = query.Where(tp => tp.TransducerModule.TransducerModuleSn!.Contains(transducerModuleSn));
            }

            if (!string.IsNullOrEmpty(transducerSn))
            {
                query = query.Where(tp => tp.TransducerModule.TransducerSn!.Contains(transducerSn));
            }

            if (!string.IsNullOrEmpty(motorModuleSn))
            {
                query = query.Where(tp => tp.MotorModule.MotorModuleSn!.Contains(motorModuleSn));
            }

            return query.ToList();
        }

        public async Task<List<TestProbe>> GetTestProbeAsync(DateTime? startDate, DateTime? endDate, int? categoryId, int? testTypeId, string? tester, int? pcId, int? result, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            IQueryable<TestProbe> query =
                (from t in _context.Set<Test>()
                 join p in _context.Set<Probe>() on t.TransducerModuleId equals p.TransducerModuleId into pGroup
                 from p in pGroup.DefaultIfEmpty()
                 select new TestProbe
                 {
                     Id = t.Id,
                     CreatedDate = t.CreatedDate,
                     Detail = t.Detail,
                     Category = t.Category,
                     TestType = t.TestType,
                     Tester = t.Tester.Name,
                     Pc = t.Tester.Pc,
                     OriginalImg = t.OriginalImg,
                     ChangedImg = t.ChangedImg,
                     ChangedImgMetadata = t.ChangedImgMetadata,
                     Result = t.Result,
                     Method = t.Method,
                     TransducerModule = t.TransducerModule,
                     ProbeSn = p.ProbeSn,
                     MotorModule = p.MotorModule
                 });

            if (startDate != null)
            {
                query = query.Where(tp => tp.CreatedDate >= startDate);
            }

            if (endDate != null)
            {
                query = query.Where(tp => tp.CreatedDate <= endDate);
            }

            if (categoryId != null && categoryId != 0)
            {
                query = query.Where(tp => tp.Category.Id == categoryId);
            }

            if (testTypeId != null && testTypeId != 0)
            {
                query = query.Where(tp => tp.TestType.Id == testTypeId);
            }

            if (!string.IsNullOrEmpty(tester))
            {
                query = query.Where(tp => tp.Tester == tester);
            }

            if (pcId != null && pcId != 0)
            {
                query = query.Where(tp => tp.Pc.Id == pcId);
            }

            if (result != null && result != 0)
            {
                if (result == 1)
                {
                    query = query.Where(tp => tp.Result > 2);
                }
                else if (result == 2)
                {
                    query = query.Where(tp => tp.Result == 2);
                }
            }

            if (!string.IsNullOrEmpty(probeSn))
            {
                query = query.Where(tp => tp.ProbeSn!.Contains(probeSn));
            }

            if (!string.IsNullOrEmpty(transducerModuleSn))
            {
                query = query.Where(tp => tp.TransducerModule.TransducerModuleSn!.Contains(transducerModuleSn));
            }

            if (!string.IsNullOrEmpty(transducerSn))
            {
                query = query.Where(tp => tp.TransducerModule.TransducerSn!.Contains(transducerSn));
            }

            if (!string.IsNullOrEmpty(motorModuleSn))
            {
                query = query.Where(tp => tp.MotorModule.MotorModuleSn!.Contains(motorModuleSn));
            }

            return await query.ToListAsync();
        }

    }
}
