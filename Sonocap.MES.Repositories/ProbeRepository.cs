using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;
using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Repositories.Interfaces;

namespace SonoCap.MES.Repositories
{
    public class ProbeRepository : RepositoryBase<Probe>, IProbeRepository
    {
        private string ProbeTestResultsQuery1 =
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

        private string ProbeTestResultsQuery = @"
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
    (SELECT * FROM Tests WHERE Id IN (
        SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 1 AND TestTypeId = 1 AND DataFlag = 1 GROUP BY TransducerId
    )) t1 ON td.Id = t1.TransducerId
LEFT JOIN
    (SELECT * FROM Tests WHERE Id IN (
        SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 1 AND TestTypeId = 2 AND DataFlag = 1 GROUP BY TransducerId
    )) t2 ON td.Id = t2.TransducerId
LEFT JOIN
    (SELECT * FROM Tests WHERE Id IN (
        SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 1 AND TestTypeId = 3 AND DataFlag = 1 GROUP BY TransducerId
    )) t3 ON td.Id = t3.TransducerId
LEFT JOIN
    (SELECT * FROM Tests WHERE Id IN (
        SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 2 AND TestTypeId = 1 AND DataFlag = 1 GROUP BY TransducerModuleId
    )) t4 ON tm.Id = t4.TransducerModuleId
LEFT JOIN
    (SELECT * FROM Tests WHERE Id IN (
        SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 2 AND TestTypeId = 2 AND DataFlag = 1 GROUP BY TransducerModuleId
    )) t5 ON tm.Id = t5.TransducerModuleId
LEFT JOIN
    (SELECT * FROM Tests WHERE Id IN (
        SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 2 AND TestTypeId = 3 AND DataFlag = 1 GROUP BY TransducerModuleId
    )) t6 ON tm.Id = t6.TransducerModuleId
LEFT JOIN
    (SELECT * FROM Tests WHERE Id IN (
        SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 3 AND TestTypeId = 1 AND DataFlag = 1 GROUP BY ProbeId
    )) t7 ON p.Id = t7.ProbeId
LEFT JOIN
    (SELECT * FROM Tests WHERE Id IN (
        SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 3 AND TestTypeId = 2 AND DataFlag = 1 GROUP BY ProbeId
    )) t8 ON p.Id = t8.ProbeId
LEFT JOIN
    (SELECT * FROM Tests WHERE Id IN (
        SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 3 AND TestTypeId = 3 AND DataFlag = 1 GROUP BY ProbeId
    )) t9 ON p.Id = t9.ProbeId
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

        public async Task<List<ProbeTestResult>> GetProbeTestResultAsync2(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
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
                    TestCreatedDate1 = t1.CreatedDate,
                    TestResult1 = t1.Result,
                    TestCategoryId2 = t2.TestCategoryId,
                    TestTypeId2 = t2.TestTypeId,
                    TestCreatedDate2 = t2.CreatedDate,
                    TestResult2 = t2.Result,
                    TestCategoryId3 = t3.TestCategoryId,
                    TestTypeId3 = t3.TestTypeId,
                    TestCreatedDate3 = t3.CreatedDate,
                    TestResult3 = t3.Result,
                    TestCategoryId4 = t4.TestCategoryId,
                    TestTypeId4 = t4.TestTypeId,
                    TestCreatedDate4 = t4.CreatedDate,
                    TestResult4 = t4.Result,
                    TestCategoryId5 = t5.TestCategoryId,
                    TestTypeId5 = t5.TestTypeId,
                    TestCreatedDate5 = t5.CreatedDate,
                    TestResult5 = t5.Result,
                    TestCategoryId6 = t6.TestCategoryId,
                    TestTypeId6 = t6.TestTypeId,
                    TestCreatedDate6 = t6.CreatedDate,
                    TestResult6 = t6.Result,
                    TestCategoryId7 = t7.TestCategoryId,
                    TestTypeId7 = t7.TestTypeId,
                    TestCreatedDate7 = t7.CreatedDate,
                    TestResult7 = t7.Result,
                    TestCategoryId8 = t8.TestCategoryId,
                    TestTypeId8 = t8.TestTypeId,
                    TestCreatedDate8 = t8.CreatedDate,
                    TestResult8 = t8.Result,
                    TestCategoryId9 = t9.TestCategoryId,
                    TestTypeId9 = t9.TestTypeId,
                    TestCreatedDate9 = t9.CreatedDate,
                    TestResult9 = t9.Result
                };

            return await query.ToListAsync();
        }
        
        public async Task<List<ProbeTestResult>> GetProbeTestResultAsync1(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            IQueryable<ProbeTestResult> query =
            from p in _context.Set<Probe>()
            join tm in _context.Set<TransducerModule>() on p.TransducerModuleId equals tm.Id into tmGroup
            from tm in tmGroup.DefaultIfEmpty()
            join td in _context.Set<Transducer>() on tm.TransducerId equals td.Id into tdGroup
            from td in tdGroup.DefaultIfEmpty()
            join mm in _context.Set<MotorModule>() on p.MotorModuleId equals mm.Id into mmGroup
            from mm in mmGroup.DefaultIfEmpty()
            join t1Grp in (
                from t in _context.Set<Test>()
                where t.TestCategoryId == 1 && t.TestTypeId == 1 && t.DataFlag == 1
                group t by t.TransducerId into g
                select g.OrderByDescending(t => t.Id).FirstOrDefault()
            ) on td.Id equals t1Grp.TransducerId into t1Group
            from t1 in t1Group.DefaultIfEmpty()
            join t2Grp in (
                from t in _context.Set<Test>()
                where t.TestCategoryId == 1 && t.TestTypeId == 2 && t.DataFlag == 1
                group t by t.TransducerId into g
                select g.OrderByDescending(t => t.Id).FirstOrDefault()
            ) on td.Id equals t2Grp.TransducerId into t2Group
            from t2 in t2Group.DefaultIfEmpty()
            join t3Grp in (
                from t in _context.Set<Test>()
                where t.TestCategoryId == 1 && t.TestTypeId == 3 && t.DataFlag == 1
                group t by t.TransducerId into g
                select g.OrderByDescending(t => t.Id).FirstOrDefault()
            ) on td.Id equals t3Grp.TransducerId into t3Group
            from t3 in t3Group.DefaultIfEmpty()
            join t4Grp in (
                from t in _context.Set<Test>()
                where t.TestCategoryId == 2 && t.TestTypeId == 1 && t.DataFlag == 1
                group t by t.TransducerModuleId into g
                select g.OrderByDescending(t => t.Id).FirstOrDefault()
            ) on tm.Id equals t4Grp.TransducerModuleId into t4Group
            from t4 in t4Group.DefaultIfEmpty()
            join t5Grp in (
                from t in _context.Set<Test>()
                where t.TestCategoryId == 2 && t.TestTypeId == 2 && t.DataFlag == 1
                group t by t.TransducerModuleId into g
                select g.OrderByDescending(t => t.Id).FirstOrDefault()
            ) on tm.Id equals t5Grp.TransducerModuleId into t5Group
            from t5 in t5Group.DefaultIfEmpty()
            join t6Grp in (
                from t in _context.Set<Test>()
                where t.TestCategoryId == 2 && t.TestTypeId == 3 && t.DataFlag == 1
                group t by t.TransducerModuleId into g
                select g.OrderByDescending(t => t.Id).FirstOrDefault()
            ) on tm.Id equals t6Grp.TransducerModuleId into t6Group
            from t6 in t6Group.DefaultIfEmpty()
            join t7Grp in (
                from t in _context.Set<Test>()
                where t.TestCategoryId == 3 && t.TestTypeId == 1 && t.DataFlag == 1
                group t by t.ProbeId into g
                select g.OrderByDescending(t => t.Id).FirstOrDefault()
            ) on p.Id equals t7Grp.ProbeId into t7Group
            from t7 in t7Group.DefaultIfEmpty()
            join t8Grp in (
                from t in _context.Set<Test>()
                where t.TestCategoryId == 3 && t.TestTypeId == 2 && t.DataFlag == 1
                group t by t.ProbeId into g
                select g.OrderByDescending(t => t.Id).FirstOrDefault()
            ) on p.Id equals t8Grp.ProbeId into t8Group
            from t8 in t8Group.DefaultIfEmpty()
            join t9Grp in (
                from t in _context.Set<Test>()
                where t.TestCategoryId == 3 && t.TestTypeId == 3 && t.DataFlag == 1
                group t by t.ProbeId into g
                select g.OrderByDescending(t => t.Id).FirstOrDefault()
            ) on p.Id equals t9Grp.ProbeId into t9Group
            from t9 in t9Group.DefaultIfEmpty()
            where
                p.DataFlag == 1 &&
                (!startDate.HasValue || p.CreatedDate >= startDate) &&
                (!endDate.HasValue || p.CreatedDate <= endDate) &&
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
                TestCreatedDate1 = t1.CreatedDate,
                TestResult1 = t1.Result,
                TestCategoryId2 = t2.TestCategoryId,
                TestTypeId2 = t2.TestTypeId,
                TestCreatedDate2 = t2.CreatedDate,
                TestResult2 = t2.Result,
                TestCategoryId3 = t3.TestCategoryId,
                TestTypeId3 = t3.TestTypeId,
                TestCreatedDate3 = t3.CreatedDate,
                TestResult3 = t3.Result,
                TestCategoryId4 = t4.TestCategoryId,
                TestTypeId4 = t4.TestTypeId,
                TestCreatedDate4 = t4.CreatedDate,
                TestResult4 = t4.Result,
                TestCategoryId5 = t5.TestCategoryId,
                TestTypeId5 = t5.TestTypeId,
                TestCreatedDate5 = t5.CreatedDate,
                TestResult5 = t5.Result,
                TestCategoryId6 = t6.TestCategoryId,
                TestTypeId6 = t6.TestTypeId,
                TestCreatedDate6 = t6.CreatedDate,
                TestResult6 = t6.Result,
                TestCategoryId7 = t7.TestCategoryId,
                TestTypeId7 = t7.TestTypeId,
                TestCreatedDate7 = t7.CreatedDate,
                TestResult7 = t7.Result,
                TestCategoryId8 = t8.TestCategoryId,
                TestTypeId8 = t8.TestTypeId,
                TestCreatedDate8 = t8.CreatedDate,
                TestResult8 = t8.Result,
                TestCreatedDate9 = t9.CreatedDate,
                TestResult9 = t9.Result
            };
            return await query.ToListAsync();
        }

        public async Task<List<ProbeTestResult>> GetProbeTestResultAsync0(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            var query = 
                from p in _context.Set<Probe>()
                join tm in _context.Set<TransducerModule>().Where(tm => tm.DataFlag == 1) on p.TransducerModuleId equals tm.Id into tmGroup
                from tm in tmGroup.DefaultIfEmpty()
                join td in _context.Set<Transducer>().Where(td => td.DataFlag == 1) on tm.TransducerId equals td.Id into tdGroup
                from td in tdGroup.DefaultIfEmpty()
                join mm in _context.Set<MotorModule>().Where(mm => mm.DataFlag == 1) on p.MotorModuleId equals mm.Id into mmGroup
                from mm in mmGroup.DefaultIfEmpty()
                join t1 in (
                    from t in _context.Set<Test>()
                    where t.TestCategoryId == 1 && t.TestTypeId == 1 && t.DataFlag == 1
                    group t by t.TransducerId into g
                    select g.OrderByDescending(t => t.Id).FirstOrDefault()
                ) on td.Id equals t1.TransducerId into t1Group
                from t1 in t1Group.DefaultIfEmpty()
                join t2 in (
                    from t in _context.Set<Test>()
                    where t.TestCategoryId == 1 && t.TestTypeId == 2 && t.DataFlag == 1
                    group t by t.TransducerId into g
                    select g.OrderByDescending(t => t.Id).FirstOrDefault()
                ) on td.Id equals t2.TransducerId into t2Group
                from t2 in t2Group.DefaultIfEmpty()
                join t3 in (
                    from t in _context.Set<Test>()
                    where t.TestCategoryId == 1 && t.TestTypeId == 3 && t.DataFlag == 1
                    group t by t.TransducerId into g
                    select g.OrderByDescending(t => t.Id).FirstOrDefault()
                ) on td.Id equals t3.TransducerId into t3Group
                from t3 in t3Group.DefaultIfEmpty()
                join t4 in (
                    from t in _context.Set<Test>()
                    where t.TestCategoryId == 2 && t.TestTypeId == 1 && t.DataFlag == 1
                    group t by t.TransducerModuleId into g
                    select g.OrderByDescending(t => t.Id).FirstOrDefault()
                ) on tm.Id equals t4.TransducerModuleId into t4Group
                from t4 in t4Group.DefaultIfEmpty()
                join t5 in (
                    from t in _context.Set<Test>()
                    where t.TestCategoryId == 2 && t.TestTypeId == 2 && t.DataFlag == 1
                    group t by t.TransducerModuleId into g
                    select g.OrderByDescending(t => t.Id).FirstOrDefault()
                ) on tm.Id equals t5.TransducerModuleId into t5Group
                from t5 in t5Group.DefaultIfEmpty()
                join t6 in (
                    from t in _context.Set<Test>()
                    where t.TestCategoryId == 2 && t.TestTypeId == 3 && t.DataFlag == 1
                    group t by t.TransducerModuleId into g
                    select g.OrderByDescending(t => t.Id).FirstOrDefault()
                ) on tm.Id equals t6.TransducerModuleId into t6Group
                from t6 in t6Group.DefaultIfEmpty()
                join t7 in (
                    from t in _context.Set<Test>()
                    where t.TestCategoryId == 3 && t.TestTypeId == 1 && t.DataFlag == 1
                    group t by t.ProbeId into g
                    select g.OrderByDescending(t => t.Id).FirstOrDefault()
                ) on p.Id equals t7.ProbeId into t7Group
                from t7 in t7Group.DefaultIfEmpty()
                join t8 in (
                    from t in _context.Set<Test>()
                    where t.TestCategoryId == 3 && t.TestTypeId == 2 && t.DataFlag == 1
                    group t by t.ProbeId into g
                    select g.OrderByDescending(t => t.Id).FirstOrDefault()
                ) on p.Id equals t8.ProbeId into t8Group
                from t8 in t8Group.DefaultIfEmpty()
                join t9 in (
                    from t in _context.Set<Test>()
                    where t.TestCategoryId == 3 && t.TestTypeId == 3 && t.DataFlag == 1
                    group t by t.ProbeId into g
                    select g.OrderByDescending(t => t.Id).FirstOrDefault()
                ) on p.Id equals t9.ProbeId into t9Group
                from t9 in t9Group.DefaultIfEmpty()
                where p.DataFlag == 1 &&
                        (!startDate.HasValue || p.CreatedDate >= startDate) &&
                        (!endDate.HasValue || p.CreatedDate <= endDate) &&
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
                    TestCreatedDate1 = t1.CreatedDate,
                    TestResult1 = t1.Result,
                    TestCategoryId2 = t2.TestCategoryId,
                    TestTypeId2 = t2.TestTypeId,
                    TestCreatedDate2 = t2.CreatedDate,
                    TestResult2 = t2.Result,
                    TestCategoryId3 = t3.TestCategoryId,
                    TestTypeId3 = t3.TestTypeId,
                    TestCreatedDate3 = t3.CreatedDate,
                    TestResult3 = t3.Result,
                    TestCategoryId4 = t4.TestCategoryId,
                    TestTypeId4 = t4.TestTypeId,
                    TestCreatedDate4 = t4.CreatedDate,
                    TestResult4 = t4.Result,
                    TestCategoryId5 = t5.TestCategoryId,
                    TestTypeId5 = t5.TestTypeId,
                    TestCreatedDate5 = t5.CreatedDate,
                    TestResult5 = t5.Result,
                    TestCategoryId6 = t6.TestCategoryId,
                    TestTypeId6 = t6.TestTypeId,
                    TestCreatedDate6 = t6.CreatedDate,
                    TestResult6 = t6.Result,
                    TestCategoryId7 = t7.TestCategoryId,
                    TestTypeId7 = t7.TestTypeId,
                    TestCreatedDate7 = t7.CreatedDate,
                    TestResult7 = t7.Result,
                    TestCategoryId8 = t8.TestCategoryId,
                    TestTypeId8 = t8.TestTypeId,
                    TestCreatedDate8 = t8.CreatedDate,
                    TestResult8 = t8.Result,
                    TestCategoryId9 = t9.TestCategoryId,
                    TestTypeId9 = t9.TestTypeId,
                    TestCreatedDate9 = t9.CreatedDate,
                    TestResult9 = t9.Result
                };

            return await query.ToListAsync();
        }

        public async Task<List<ProbeTestResult>> GetProbeTestResultAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            var query = from p in _context.Set<Probe>()
                        join tm in _context.Set<TransducerModule>().Where(tm => tm.DataFlag == 1) on p.TransducerModuleId equals tm.Id into tmGroup
                        from tm in tmGroup.DefaultIfEmpty()
                        join td in _context.Set<Transducer>().Where(td => td.DataFlag == 1) on tm.TransducerId equals td.Id into tdGroup
                        from td in tdGroup.DefaultIfEmpty()
                        join mm in _context.Set<MotorModule>().Where(mm => mm.DataFlag == 1) on p.MotorModuleId equals mm.Id into mmGroup
                        from mm in mmGroup.DefaultIfEmpty()
                        where p.DataFlag == 1 &&
                              (!startDate.HasValue || p.CreatedDate >= startDate) &&
                              (!endDate.HasValue || p.CreatedDate <= endDate) &&
                              (string.IsNullOrEmpty(probeSn) || p.ProbeSn.Contains(probeSn)) &&
                              (string.IsNullOrEmpty(transducerModuleSn) || tm.TransducerModuleSn.Contains(transducerModuleSn)) &&
                              (string.IsNullOrEmpty(transducerSn) || td.TransducerSn.Contains(transducerSn)) &&
                              (string.IsNullOrEmpty(motorModuleSn) || mm.MotorModuleSn.Contains(motorModuleSn))
                        select new
                        {
                            p.Id,
                            p.ProbeSn,
                            p.CreatedDate,
                            TransducerModuleSn = tm.TransducerModuleSn,
                            TransducerSn = td.TransducerSn,
                            MotorModuleSn = mm.MotorModuleSn,
                            TransducerId = td.Id,
                            TransducerModuleId = tm.Id,
                            ProbeId = p.Id
                        };

            var result = await query.ToListAsync();

            var probeTestResults = result.Select(p => new ProbeTestResult
            {
                Id = p.Id,
                ProbeSn = p.ProbeSn,
                CreatedDate = p.CreatedDate,
                TransducerModuleSn = p.TransducerModuleSn,
                TransducerSn = p.TransducerSn,
                MotorModuleSn = p.MotorModuleSn,
                TestCategoryId1 = _context.Set<Test>().Where(t => t.TransducerId == p.TransducerId && t.TestCategoryId == 1 && t.TestTypeId == 1 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestCategoryId).FirstOrDefault(),
                TestTypeId1 = _context.Set<Test>().Where(t => t.TransducerId == p.TransducerId && t.TestCategoryId == 1 && t.TestTypeId == 1 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestTypeId).FirstOrDefault(),
                TestCreatedDate1 = _context.Set<Test>().Where(t => t.TransducerId == p.TransducerId && t.TestCategoryId == 1 && t.TestTypeId == 1 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.CreatedDate).FirstOrDefault(),
                TestResult1 = _context.Set<Test>().Where(t => t.TransducerId == p.TransducerId && t.TestCategoryId == 1 && t.TestTypeId == 1 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.Result).FirstOrDefault(),
                TestCategoryId2 = _context.Set<Test>().Where(t => t.TransducerId == p.TransducerId && t.TestCategoryId == 1 && t.TestTypeId == 2 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestCategoryId).FirstOrDefault(),
                TestTypeId2 = _context.Set<Test>().Where(t => t.TransducerId == p.TransducerId && t.TestCategoryId == 1 && t.TestTypeId == 2 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestTypeId).FirstOrDefault(),
                TestCreatedDate2 = _context.Set<Test>().Where(t => t.TransducerId == p.TransducerId && t.TestCategoryId == 1 && t.TestTypeId == 2 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.CreatedDate).FirstOrDefault(),
                TestResult2 = _context.Set<Test>().Where(t => t.TransducerId == p.TransducerId && t.TestCategoryId == 1 && t.TestTypeId == 2 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.Result).FirstOrDefault(),
                TestCategoryId3 = _context.Set<Test>().Where(t => t.TransducerId == p.TransducerId && t.TestCategoryId == 1 && t.TestTypeId == 3 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestCategoryId).FirstOrDefault(),
                TestTypeId3 = _context.Set<Test>().Where(t => t.TransducerId == p.TransducerId && t.TestCategoryId == 1 && t.TestTypeId == 3 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestTypeId).FirstOrDefault(),
                TestCreatedDate3 = _context.Set<Test>().Where(t => t.TransducerId == p.TransducerId && t.TestCategoryId == 1 && t.TestTypeId == 3 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.CreatedDate).FirstOrDefault(),
                TestResult3 = _context.Set<Test>().Where(t => t.TransducerId == p.TransducerId && t.TestCategoryId == 1 && t.TestTypeId == 3 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.Result).FirstOrDefault(),

                TestCategoryId4 = _context.Set<Test>().Where(t => t.TransducerModuleId == p.TransducerModuleId && t.TestCategoryId == 2 && t.TestTypeId == 1 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestCategoryId).FirstOrDefault(),
                TestTypeId4 = _context.Set<Test>().Where(t => t.TransducerModuleId == p.TransducerModuleId && t.TestCategoryId == 2 && t.TestTypeId == 1 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestTypeId).FirstOrDefault(),
                TestCreatedDate4 = _context.Set<Test>().Where(t => t.TransducerModuleId == p.TransducerModuleId && t.TestCategoryId == 2 && t.TestTypeId == 1 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.CreatedDate).FirstOrDefault(),
                TestResult4 = _context.Set<Test>().Where(t => t.TransducerModuleId == p.TransducerModuleId && t.TestCategoryId == 2 && t.TestTypeId == 1 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.Result).FirstOrDefault(),
                TestCategoryId5 = _context.Set<Test>().Where(t => t.TransducerModuleId == p.TransducerModuleId && t.TestCategoryId == 2 && t.TestTypeId == 2 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestCategoryId).FirstOrDefault(),
                TestTypeId5 = _context.Set<Test>().Where(t => t.TransducerModuleId == p.TransducerModuleId && t.TestCategoryId == 2 && t.TestTypeId == 2 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestTypeId).FirstOrDefault(),
                TestCreatedDate5 = _context.Set<Test>().Where(t => t.TransducerModuleId == p.TransducerModuleId && t.TestCategoryId == 2 && t.TestTypeId == 2 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.CreatedDate).FirstOrDefault(),
                TestResult5 = _context.Set<Test>().Where(t => t.TransducerModuleId == p.TransducerModuleId && t.TestCategoryId == 2 && t.TestTypeId == 2 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.Result).FirstOrDefault(),
                TestCategoryId6 = _context.Set<Test>().Where(t => t.TransducerModuleId == p.TransducerModuleId && t.TestCategoryId == 2 && t.TestTypeId == 3 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestCategoryId).FirstOrDefault(),
                TestTypeId6 = _context.Set<Test>().Where(t => t.TransducerModuleId == p.TransducerModuleId && t.TestCategoryId == 2 && t.TestTypeId == 3 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestTypeId).FirstOrDefault(),
                TestCreatedDate6 = _context.Set<Test>().Where(t => t.TransducerModuleId == p.TransducerModuleId && t.TestCategoryId == 2 && t.TestTypeId == 3 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.CreatedDate).FirstOrDefault(),
                TestResult6 = _context.Set<Test>().Where(t => t.TransducerModuleId == p.TransducerModuleId && t.TestCategoryId == 2 && t.TestTypeId == 3 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.Result).FirstOrDefault(),

                TestCategoryId7 = _context.Set<Test>().Where(t => t.ProbeId == p.ProbeId && t.TestCategoryId == 3 && t.TestTypeId == 1 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestCategoryId).FirstOrDefault(),
                TestTypeId7 = _context.Set<Test>().Where(t => t.ProbeId == p.ProbeId && t.TestCategoryId == 3 && t.TestTypeId == 1 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestTypeId).FirstOrDefault(),
                TestCreatedDate7 = _context.Set<Test>().Where(t => t.ProbeId == p.ProbeId && t.TestCategoryId == 3 && t.TestTypeId == 1 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.CreatedDate).FirstOrDefault(),
                TestResult7 = _context.Set<Test>().Where(t => t.ProbeId == p.ProbeId && t.TestCategoryId == 3 && t.TestTypeId == 1 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.Result).FirstOrDefault(),
                TestCategoryId8 = _context.Set<Test>().Where(t => t.ProbeId == p.ProbeId && t.TestCategoryId == 3 && t.TestTypeId == 2 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestCategoryId).FirstOrDefault(),
                TestTypeId8 = _context.Set<Test>().Where(t => t.ProbeId == p.ProbeId && t.TestCategoryId == 3 && t.TestTypeId == 2 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestTypeId).FirstOrDefault(),
                TestCreatedDate8 = _context.Set<Test>().Where(t => t.ProbeId == p.ProbeId && t.TestCategoryId == 3 && t.TestTypeId == 2 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.CreatedDate).FirstOrDefault(),
                TestResult8 = _context.Set<Test>().Where(t => t.ProbeId == p.ProbeId && t.TestCategoryId == 3 && t.TestTypeId == 2 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.Result).FirstOrDefault(),
                TestCategoryId9 = _context.Set<Test>().Where(t => t.ProbeId == p.ProbeId && t.TestCategoryId == 3 && t.TestTypeId == 3 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestCategoryId).FirstOrDefault(),
                TestTypeId9 = _context.Set<Test>().Where(t => t.ProbeId == p.ProbeId && t.TestCategoryId == 3 && t.TestTypeId == 3 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.TestTypeId).FirstOrDefault(),
                TestCreatedDate9 = _context.Set<Test>().Where(t => t.ProbeId == p.ProbeId && t.TestCategoryId == 3 && t.TestTypeId == 3 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.CreatedDate).FirstOrDefault(),
                TestResult9 = _context.Set<Test>().Where(t => t.ProbeId == p.ProbeId && t.TestCategoryId == 3 && t.TestTypeId == 3 && t.DataFlag == 1).OrderByDescending(t => t.Id).Select(t => t.Result).FirstOrDefault(),
            }).ToList();
            
            return probeTestResults;
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

        //todo 미확인
        public async Task<List<ProbeTestResult>> GetProbeTestResultSqlAsync2(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            var parameters = new List<MySqlParameter>
    {
        new MySqlParameter("@StartDate", startDate ?? (object)DBNull.Value),
        new MySqlParameter("@EndDate", endDate ?? (object)DBNull.Value),
        new MySqlParameter("@ProbeSn", probeSn ?? (object)DBNull.Value),
        new MySqlParameter("@TransducerModuleSn", transducerModuleSn ?? (object)DBNull.Value),
        new MySqlParameter("@TransducerSn", transducerSn ?? (object)DBNull.Value),
        new MySqlParameter("@MotorModuleSn", motorModuleSn ?? (object)DBNull.Value)
    };

            string query = @"
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
        (SELECT * FROM Tests WHERE Id IN (
            SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 1 AND TestTypeId = 1 AND DataFlag = 1 GROUP BY TransducerId
        )) t1 ON td.Id = t1.TransducerId
    LEFT JOIN
        (SELECT * FROM Tests WHERE Id IN (
            SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 1 AND TestTypeId = 2 AND DataFlag = 1 GROUP BY TransducerId
        )) t2 ON td.Id = t2.TransducerId
    LEFT JOIN
        (SELECT * FROM Tests WHERE Id IN (
            SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 1 AND TestTypeId = 3 AND DataFlag = 1 GROUP BY TransducerId
        )) t3 ON td.Id = t3.TransducerId
    LEFT JOIN
        (SELECT * FROM Tests WHERE Id IN (
            SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 2 AND TestTypeId = 1 AND DataFlag = 1 GROUP BY TransducerModuleId
        )) t4 ON tm.Id = t4.TransducerModuleId
    LEFT JOIN
        (SELECT * FROM Tests WHERE Id IN (
            SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 2 AND TestTypeId = 2 AND DataFlag = 1 GROUP BY TransducerModuleId
        )) t5 ON tm.Id = t5.TransducerModuleId
    LEFT JOIN
        (SELECT * FROM Tests WHERE Id IN (
            SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 2 AND TestTypeId = 3 AND DataFlag = 1 GROUP BY TransducerModuleId
        )) t6 ON tm.Id = t6.TransducerModuleId
    LEFT JOIN
        (SELECT * FROM Tests WHERE Id IN (
            SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 3 AND TestTypeId = 1 AND DataFlag = 1 GROUP BY ProbeId
        )) t7 ON p.Id = t7.ProbeId
    LEFT JOIN
        (SELECT * FROM Tests WHERE Id IN (
            SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 3 AND TestTypeId = 2 AND DataFlag = 1 GROUP BY ProbeId
        )) t8 ON p.Id = t8.ProbeId
    LEFT JOIN
        (SELECT * FROM Tests WHERE Id IN (
            SELECT MAX(Id) FROM Tests WHERE TestCategoryId = 3 AND TestTypeId = 3 AND DataFlag = 1 GROUP BY ProbeId
        )) t9 ON p.Id = t9.ProbeId
    WHERE
        p.DataFlag = 1 AND
        (p.CreatedDate >= @StartDate OR @StartDate IS NULL) AND
        (p.CreatedDate <= @EndDate OR @EndDate IS NULL) AND
        (p.ProbeSn = @ProbeSn OR @ProbeSn IS NULL OR @ProbeSn = '') AND
        (tm.TransducerModuleSn = @TransducerModuleSn OR @TransducerModuleSn IS NULL OR @TransducerModuleSn = '') AND
        (td.TransducerSn = @TransducerSn OR @TransducerSn IS NULL OR @TransducerSn = '') AND
        (mm.MotorModuleSn = @MotorModuleSn OR @MotorModuleSn IS NULL OR @MotorModuleSn = '')
    ";

            return await _context.Set<ProbeTestResult>().FromSqlRaw(query, parameters.ToArray()).ToListAsync();
        }

    }
}
