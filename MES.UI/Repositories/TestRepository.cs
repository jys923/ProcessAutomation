using MES.UI.Models;
using MES.UI.Models.Context;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
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
                     Category = t.Category,
                     TestType = t.TestType,
                     Tester = t.Tester,
                     ProbeSn = p.ProbeSn,
                     TransducerModule = t.TransducerModule,
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
                     Category = t.Category,
                     TestType = t.TestType,
                     Tester = t.Tester,
                     ProbeSn = p.ProbeSn,
                     TransducerModule = t.TransducerModule,
                     MotorModule = p.MotorModule
                 }).ToList();

            return testprobes;
        }

        public List<TestProbe> GetTestProbe(DateTime? startDate, DateTime? endDate, int? categoryId, int? testTypeId, int? testerId, int? pcNo, int? result, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            IQueryable<TestProbe> query =
                (from t in _context.Set<Test>()
                 join p in _context.Set<Probe>() on t.TransducerModuleId equals p.TransducerModuleId into pGroup
                 from p in pGroup.DefaultIfEmpty()
                 select new TestProbe
                 {
                     Id = t.Id,
                     CreatedDate = t.CreatedDate,
                     Category = t.Category,
                     TestType = t.TestType,
                     Tester = t.Tester,
                     ProbeSn = p.ProbeSn,
                     TransducerModule = t.TransducerModule,
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

            if (categoryId != null)
            {
                query = query.Where(tp => tp.Category.Id == categoryId);
            }

            if (testTypeId != null)
            {
                query = query.Where(tp => tp.TestType.Id == testTypeId);
            }

            if (testerId != null)
            {
                query = query.Where(tp => tp.Tester.Id == testerId);
            }

            if (pcNo != null)
            {
                query = query.Where(tp => tp.Tester.PcNo == pcNo);
            }

            if (result != null)
            {
                //query = query.Where(tp => tp.Result == result);
            }

            if (!string.IsNullOrEmpty(probeSn))
            {
                query = query.Where(tp => tp.ProbeSn == probeSn);
            }

            if (!string.IsNullOrEmpty(transducerModuleSn))
            {
                query = query.Where(tp => tp.TransducerModule.TransducerModuleSn == transducerModuleSn);
            }

            if (!string.IsNullOrEmpty(transducerSn))
            {
                query = query.Where(tp => tp.TransducerModule.TransducerSn == transducerSn);
            }

            if (!string.IsNullOrEmpty(motorModuleSn))
            {
                query = query.Where(tp => tp.MotorModule.MotorModuleSn == motorModuleSn);
            }

            return query.ToList();
        }
    }
}
