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

        public async Task<IEnumerable<Test>> GetTestAsync(
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
            IQueryable<Test> query =
                (from t in _context.Set<Test>()
                 join p in _context.Set<Probe>() on t.ProbeId equals p.Id into pGroup
                 from p in pGroup.DefaultIfEmpty()
                 join tm in _context.Set<TransducerModule>() on t.TransducerModuleId equals tm.Id into tmGroup
                 from tm in tmGroup.DefaultIfEmpty()
                 join td in _context.Set<Transducer>() on t.TransducerId equals td.Id into tdGroup
                 from td in tdGroup.DefaultIfEmpty()
                 join mm in _context.Set<MotorModule>() on p.MotorModuleId equals mm.Id into mmGroup
                 from mm in mmGroup.DefaultIfEmpty()
                 join tt in _context.Set<TestType>() on t.TestTypeId equals tt.Id into ttGroup
                 from tt in ttGroup.DefaultIfEmpty()
                 where t.DataFlag == 1
                    && t.Id < 10000//0
                    && (startDate == null || t.CreatedDate >= startDate)
                    && (endDate == null || t.CreatedDate <= endDate)
                    && (categoryId == null || t.TestCategoryId == categoryId)
                    && (testTypeId == null || t.TestTypeId == testTypeId)
                    && (string.IsNullOrEmpty(tester) || t.Tester.Name.Contains(tester))
                    && (pcId == null || t.Tester.PcId == pcId)
                    && (result == null ||
                        (result == 1 && tt != null && t.Result >= tt.Threshold) ||
                        (result != 1 && tt != null && t.Result < tt.Threshold))
                    && (dataFlagTest == null || t.DataFlag == dataFlagTest)
                    && (string.IsNullOrEmpty(probeSn) || p.Sn.Contains(probeSn))
                    && (string.IsNullOrEmpty(transducerModuleSn) || tm.Sn.Contains(transducerModuleSn))
                    && (string.IsNullOrEmpty(transducerSn) || td.Sn.Contains(transducerSn))
                    && (string.IsNullOrEmpty(motorModuleSn) || mm.Sn.Contains(motorModuleSn))
                 select t);

            return await query.ToListAsync();
        }


        public async Task<List<TestProbe>> GetTestProbeLinqAsync2(
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

        public IEnumerable<Test> GetLatestTests(Transducer? transducer = null, TransducerModule? transducerModule = null, Probe? probe = null)
        {
            // 매개변수 중 적어도 하나는 null이 아닌지 확인
            if (transducer == null && transducerModule == null && probe == null)
            {
                throw new ArgumentException("At least one of 'transducer', 'transducerModule', or 'probe' must be provided.");
            }

            IQueryable<Test> resultQuery = _context.Set<Test>();

            // 필터 조건 추가
            if (transducer != null)
            {
                resultQuery = resultQuery
                    .Where(test => test.TransducerId == transducer.Id);
            }
            else if (transducerModule != null)
            {
                resultQuery = resultQuery
                    .Where(test => test.TransducerModuleId == transducerModule.Id);
            }
            else if (probe != null)
            {
                resultQuery = resultQuery
                    .Where(test => test.ProbeId == probe.Id);
            }

            // 최신 Test 항목을 가져오기 위한 조건 추가
            if (transducer != null)
            {
                resultQuery = resultQuery
                    .Where(test => new[] { 1, 2, 3 }.Contains(test.TestTypeId)
                               && test.Id == (
                                   from t2 in _context.Set<Test>()
                                   where t2.TransducerId == transducer.Id
                                     && t2.TestTypeId == test.TestTypeId
                                   select t2.Id).Max());
            }
            else if (transducerModule != null)
            {
                resultQuery = resultQuery
                    .Where(test => new[] { 1, 2, 3 }.Contains(test.TestTypeId)
                               && test.Id == (
                                   from t2 in _context.Set<Test>()
                                   where t2.TransducerModuleId == transducerModule.Id
                                     && t2.TestTypeId == test.TestTypeId
                                   select t2.Id).Max());
            }
            else if (probe != null)
            {
                resultQuery = resultQuery
                    .Where(test => new[] { 1, 2, 3 }.Contains(test.TestTypeId)
                               && test.Id == (
                                   from t2 in _context.Set<Test>()
                                   where t2.ProbeId == probe.Id
                                     && t2.TestTypeId == test.TestTypeId
                                   select t2.Id).Max());
            }

            resultQuery = resultQuery.OrderBy(test => test.TestTypeId);

            return resultQuery.ToList();
        }

        public async Task<IEnumerable<Test>> GetLatestTestsAsync(int? transducerId = null, int? transducerModuleId = null, int? probeId = null)
        {
            // 매개변수 중 적어도 하나는 null이 아닌지 확인
            if (transducerId == null && transducerModuleId == null && probeId == null)
            {
                throw new ArgumentException("At least one of 'transducerId', 'transducerModuleId', or 'probeId' must be provided.");
            }

            IQueryable<Test> resultQuery = _context.Set<Test>();

            // ----------------- [ 필터 조건: ID 사용 ] -----------------
            if (transducerId.HasValue)
            {
                resultQuery = resultQuery
                    .Where(test => test.TransducerId == transducerId.Value);
            }
            else if (transducerModuleId.HasValue)
            {
                resultQuery = resultQuery
                    .Where(test => test.TransducerModuleId == transducerModuleId.Value);
            }
            else if (probeId.HasValue)
            {
                resultQuery = resultQuery
                    .Where(test => test.ProbeId == probeId.Value);
            }

            // ----------------- [ 최신 Test 항목 쿼리 조건: ID 사용 ] -----------------
            // 기존 GetLatestTests 로직과 동일하나, ID를 사용합니다.
            if (transducerId.HasValue)
            {
                resultQuery = resultQuery
                    .Where(test => new[] { 1, 2, 3 }.Contains(test.TestTypeId)
                                 && test.Id == (
                                     from t2 in _context.Set<Test>()
                                     where t2.TransducerId == transducerId.Value
                                       && t2.TestTypeId == test.TestTypeId
                                     select t2.Id).Max());
            }
            else if (transducerModuleId.HasValue)
            {
                resultQuery = resultQuery
                    .Where(test => new[] { 1, 2, 3 }.Contains(test.TestTypeId)
                                 && test.Id == (
                                     from t2 in _context.Set<Test>()
                                     where t2.TransducerModuleId == transducerModuleId.Value
                                       && t2.TestTypeId == test.TestTypeId
                                     select t2.Id).Max());
            }
            else if (probeId.HasValue)
            {
                resultQuery = resultQuery
                    .Where(test => new[] { 1, 2, 3 }.Contains(test.TestTypeId)
                                 && test.Id == (
                                     from t2 in _context.Set<Test>()
                                     where t2.ProbeId == probeId.Value
                                       && t2.TestTypeId == test.TestTypeId
                                     select t2.Id).Max());
            }

            resultQuery = resultQuery.OrderBy(test => test.TestTypeId);

            // ToList() 대신 비동기 메서드인 ToListAsync()를 사용하고 await를 붙입니다.
            return await resultQuery.ToListAsync();
        }
    }
}
