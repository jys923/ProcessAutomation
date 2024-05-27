using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;
using SonoCap.MES.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace SonoCap.MES.Repositories
{
    public class ProbeRepository : RepositoryBase<Probe>, IProbeRepository
    {
        private string ProbeTestResultsQuery =
            @"
            SELECT
                p.Id,
                p.ProbeSn,
                p.CreatedDate,
                tm.TransducerModuleSn,
                td.TransducerSn,
                mm.MotorModuleSn,
                1 AS TestCategoryId1, 1 AS TestTypeId1, t1.CreatedDate AS Test1_CreatedDate, t1.Result AS Test1_Result,
                1 AS TestCategoryId2, 2 AS TestTypeId2, t2.CreatedDate AS Test2_CreatedDate, t2.Result AS Test2_Result,
                1 AS TestCategoryId3, 3 AS TestTypeId3, t3.CreatedDate AS Test3_CreatedDate, t3.Result AS Test3_Result,
                2 AS TestCategoryId4, 1 AS TestTypeId4, t4.CreatedDate AS Test4_CreatedDate, t4.Result AS Test4_Result,
                2 AS TestCategoryId5, 2 AS TestTypeId5, t5.CreatedDate AS Test5_CreatedDate, t5.Result AS Test5_Result,
                2 AS TestCategoryId6, 3 AS TestTypeId6, t6.CreatedDate AS Test6_CreatedDate, t6.Result AS Test6_Result,
                3 AS TestCategoryId7, 1 AS TestTypeId7, t7.CreatedDate AS Test7_CreatedDate, t7.Result AS Test7_Result,
                3 AS TestCategoryId8, 2 AS TestTypeId8, t8.CreatedDate AS Test8_CreatedDate, t8.Result AS Test8_Result,
                3 AS TestCategoryId9, 3 AS TestTypeId9, t9.CreatedDate AS Test9_CreatedDate, t9.Result AS Test9_Result
            FROM
                Probes p
            LEFT JOIN
                TransducerModules tm ON p.TransducerModuleId = tm.Id AND tm.DataFlag = 1
            LEFT JOIN
                Transducers td ON tm.TransducerId = td.Id AND td.DataFlag = 1
            LEFT JOIN
                MotorModules mm ON p.MotorModuleId = mm.Id AND mm.DataFlag = 1
            LEFT JOIN
                Tests t1 ON td.Id = t1.TransducerId AND t1.TestCategoryId = 1 AND t1.TestTypeId = 1 AND t1.DataFlag = 1 
            LEFT JOIN
                Tests t2 ON td.Id = t2.TransducerId AND t2.TestCategoryId = 1 AND t2.TestTypeId = 2 AND t2.DataFlag = 1
            LEFT JOIN
                Tests t3 ON td.Id = t3.TransducerId AND t3.TestCategoryId = 1 AND t3.TestTypeId = 3 AND t3.DataFlag = 1
            LEFT JOIN
                Tests t4 ON tm.Id = t4.TransducerModuleId AND t4.TestCategoryId = 2 AND t4.TestTypeId = 1 AND t4.DataFlag = 1
            LEFT JOIN
                Tests t5 ON tm.Id = t5.TransducerModuleId AND t5.TestCategoryId = 2 AND t5.TestTypeId = 2 AND t5.DataFlag = 1
            LEFT JOIN
                Tests t6 ON tm.Id = t6.TransducerModuleId AND t6.TestCategoryId = 2 AND t6.TestTypeId = 3 AND t6.DataFlag = 1
            LEFT JOIN
                Tests t7 ON p.Id = t7.ProbeId AND t7.TestCategoryId = 3 AND t7.TestTypeId = 1 AND t7.DataFlag = 1
            LEFT JOIN
                Tests t8 ON p.Id = t8.ProbeId AND t8.TestCategoryId = 3 AND t8.TestTypeId = 2 AND t8.DataFlag = 1
            LEFT JOIN
                Tests t9 ON p.Id = t9.ProbeId AND t9.TestCategoryId = 3 AND t9.TestTypeId = 3 AND t9.DataFlag = 1
            WHERE
                p.DataFlag = 1 AND
                (p.CreatedDate >= @StartDate OR @StartDate IS NULL) AND
                (p.CreatedDate <= @EndDate OR @EndDate IS NULL) AND
                (p.ProbeSn = @ProbeSn OR @ProbeSn IS NULL OR @ProbeSn = '') AND
                (tm.TransducerModuleSn = @TransducerModuleSn OR @TransducerModuleSn IS NULL OR @TransducerModuleSn = '') AND
                (td.TransducerSn = @TransducerSn OR @TransducerSn IS NULL OR @TransducerSn = '') AND
                (mm.MotorModuleSn = @MotorModuleSn OR @MotorModuleSn IS NULL OR @MotorModuleSn = '')
";

        public ProbeRepository(MESDbContext context) : base(context)
        {
        }

        public async Task<List<ProbeTestResult>> GetProbeTestResultAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            var query =
                from p in _context.Set<Probe>()
                join tm in _context.Set<TransducerModule>() on p.TransducerModuleId equals tm.Id into tmGroup
                from tm in tmGroup.DefaultIfEmpty()
                join td in _context.Set<Transducer>() on tm.TransducerId equals td.Id into tdGroup
                from td in tdGroup.DefaultIfEmpty()
                join mm in _context.Set<MotorModule>() on p.MotorModuleId equals mm.Id into mmGroup
                from mm in mmGroup.DefaultIfEmpty()
                join t1 in _context.Set<Test>().Where(t => t.TestCategoryId == 1 && t.TestTypeId == 1 && t.DataFlag == 1) on td.Id equals t1.TransducerId into t1Group
                from t1 in t1Group.DefaultIfEmpty()
                join t2 in _context.Set<Test>().Where(t => t.TestCategoryId == 1 && t.TestTypeId == 2 && t.DataFlag == 1) on td.Id equals t2.TransducerId into t2Group
                from t2 in t2Group.DefaultIfEmpty()
                join t3 in _context.Set<Test>().Where(t => t.TestCategoryId == 1 && t.TestTypeId == 3 && t.DataFlag == 1) on td.Id equals t3.TransducerId into t3Group
                from t3 in t3Group.DefaultIfEmpty()
                join t4 in _context.Set<Test>().Where(t => t.TestCategoryId == 2 && t.TestTypeId == 1 && t.DataFlag == 1) on tm.Id equals t4.TransducerModuleId into t4Group
                from t4 in t4Group.DefaultIfEmpty()
                join t5 in _context.Set<Test>().Where(t => t.TestCategoryId == 2 && t.TestTypeId == 2 && t.DataFlag == 1) on tm.Id equals t5.TransducerModuleId into t5Group
                from t5 in t5Group.DefaultIfEmpty()
                join t6 in _context.Set<Test>().Where(t => t.TestCategoryId == 2 && t.TestTypeId == 3 && t.DataFlag == 1) on tm.Id equals t6.TransducerModuleId into t6Group
                from t6 in t6Group.DefaultIfEmpty()
                join t7 in _context.Set<Test>().Where(t => t.TestCategoryId == 3 && t.TestTypeId == 1 && t.DataFlag == 1) on p.Id equals t7.ProbeId into t7Group
                from t7 in t7Group.DefaultIfEmpty()
                join t8 in _context.Set<Test>().Where(t => t.TestCategoryId == 3 && t.TestTypeId == 2 && t.DataFlag == 1) on p.Id equals t8.ProbeId into t8Group
                from t8 in t8Group.DefaultIfEmpty()
                join t9 in _context.Set<Test>().Where(t => t.TestCategoryId == 3 && t.TestTypeId == 3 && t.DataFlag == 1) on p.Id equals t9.ProbeId into t9Group
                from t9 in t9Group.DefaultIfEmpty()
                where
                    p.DataFlag == 1 &&
                    (startDate == null || p.CreatedDate >= startDate) &&
                    (endDate == null || p.CreatedDate <= endDate) &&
                    (string.IsNullOrEmpty(probeSn) || p.ProbeSn.Contains(probeSn)) &&
                    (string.IsNullOrEmpty(transducerModuleSn) || tm.TransducerModuleSn.Contains(transducerModuleSn)) &&
                    (string.IsNullOrEmpty(transducerSn) || td.TransducerSn.Contains(transducerSn)) &&
                    (string.IsNullOrEmpty(motorModuleSn) || mm.MotorModuleSn.Contains(motorModuleSn))
                select new ProbeTestResult
                {
                    Id = p.Id,
                    ProbeSn = p.ProbeSn,
                    CreatedDate = p.CreatedDate,
                    TransducerModuleSn = tm.TransducerModuleSn,
                    TransducerSn = td.TransducerSn,
                    MotorModuleSn = mm.MotorModuleSn,
                    TestCategoryId1 = t1.TestCategoryId,
                    TestTypeId1 = t1.TestTypeId,
                    Test1_CreatedDate = t1.CreatedDate,
                    Test1_Result = t1.Result,
                    TestCategoryId2 = t2.TestCategoryId,
                    TestTypeId2 = t2.TestTypeId,
                    Test2_CreatedDate = t2.CreatedDate,
                    Test2_Result = t2.Result,
                    TestCategoryId3 = t3.TestCategoryId,
                    TestTypeId3 = t3.TestTypeId,
                    Test3_CreatedDate = t3.CreatedDate,
                    Test3_Result = t3.Result,
                    TestCategoryId4 = t4.TestCategoryId,
                    TestTypeId4 = t4.TestTypeId,
                    Test4_CreatedDate = t4.CreatedDate,
                    Test4_Result = t4.Result,
                    TestCategoryId5 = t5.TestCategoryId,
                    TestTypeId5 = t5.TestTypeId,
                    Test5_CreatedDate = t5.CreatedDate,
                    Test5_Result = t5.Result,
                    TestCategoryId6 = t6.TestCategoryId,
                    TestTypeId6 = t6.TestTypeId,
                    Test6_CreatedDate = t6.CreatedDate,
                    Test6_Result = t6.Result,
                    TestCategoryId7 = t7.TestCategoryId,
                    TestTypeId7 = t7.TestTypeId,
                    Test7_CreatedDate = t7.CreatedDate,
                    Test7_Result = t7.Result,
                    TestCategoryId8 = t8.TestCategoryId,
                    TestTypeId8 = t8.TestTypeId,
                    Test8_CreatedDate = t8.CreatedDate,
                    Test8_Result = t8.Result,
                    TestCategoryId9 = t9.TestCategoryId,
                    TestTypeId9 = t9.TestTypeId,
                    Test9_CreatedDate = t9.CreatedDate,
                    Test9_Result = t9.Result
                };

            return await query.ToListAsync();
        }

        public async Task<List<ProbeTestResult>> GetProbeTestResultSqlAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            List<ProbeTestResult> probeTestResultDaos = await _context.Set<ProbeTestResult>()
                .FromSqlRaw(ProbeTestResultsQuery,
                    new MySqlParameter("@StartDate", startDate),
                    new MySqlParameter("@EndDate", endDate),
                    new MySqlParameter("@ProbeSn", probeSn),
                    new MySqlParameter("@TransducerModuleSn", transducerModuleSn),
                    new MySqlParameter("@TransducerSn", transducerSn),
                    new MySqlParameter("@MotorModuleSn ", motorModuleSn)
                )
                .ToListAsync();
            return probeTestResultDaos;
        }
    }
}
