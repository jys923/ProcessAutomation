using MES.UI.Models;
using MES.UI.Models.Context;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    public class TestRepository : RepositoryBase<Test> , ITestRepository
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
    }
}
