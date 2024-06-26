using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;
using SonoCap.MES.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace SonoCap.MES.Repositories
{
    public class PTRViewRepository : RepositoryBase<PTRView>, IPTRViewRepository
    {
        public PTRViewRepository(MESDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        private string PTRViewQuery =
@"
SELECT
    ptrv.Id,
    ptrv.ProbeSn,
    ptrv.CreatedDate,
    ptrv.TransducerModuleSn,
    ptrv.TransducerSn,
    ptrv.MotorModuleSn,
    t1.TestCategoryId AS TestCategoryId1,
    t1.TestTypeId AS TestTypeId1,
    t1.CreatedDate AS TestCreatedDate1,
    t1.Result AS TestResult1,
    t2.TestCategoryId AS TestCategoryId2,
    t2.TestTypeId AS TestTypeId2,
    t2.CreatedDate AS TestCreatedDate2,
    t2.Result AS TestResult2,
    t3.TestCategoryId AS TestCategoryId3,
    t3.TestTypeId AS TestTypeId3,
    t3.CreatedDate AS TestCreatedDate3,
    t3.Result AS TestResult3,
    t4.TestCategoryId AS TestCategoryId4,
    t4.TestTypeId AS TestTypeId4,
    t4.CreatedDate AS TestCreatedDate4,
    t4.Result AS TestResult4,
    t5.TestCategoryId AS TestCategoryId5,
    t5.TestTypeId AS TestTypeId5,
    t5.CreatedDate AS TestCreatedDate5,
    t5.Result AS TestResult5,
    t6.TestCategoryId AS TestCategoryId6,
    t6.TestTypeId AS TestTypeId6,
    t6.CreatedDate AS TestCreatedDate6,
    t6.Result AS TestResult6,
    t7.TestCategoryId AS TestCategoryId7,
    t7.TestTypeId AS TestTypeId7,
    t7.CreatedDate AS TestCreatedDate7,
    t7.Result AS TestResult7,
    t8.TestCategoryId AS TestCategoryId8,
    t8.TestTypeId AS TestTypeId8,
    t8.CreatedDate AS TestCreatedDate8,
    t8.Result AS TestResult8,
    t9.TestCategoryId AS TestCategoryId9,
    t9.TestTypeId AS TestTypeId9,
    t9.CreatedDate AS TestCreatedDate9,
    t9.Result AS TestResult9
FROM 
    PTRViews ptrv
LEFT JOIN 
    Tests t1 ON ptrv.TestId01 = t1.Id
LEFT JOIN 
    Tests t2 ON ptrv.TestId02 = t2.Id
LEFT JOIN 
    Tests t3 ON ptrv.TestId03 = t3.Id
LEFT JOIN 
    Tests t4 ON ptrv.TestId04 = t4.Id
LEFT JOIN 
    Tests t5 ON ptrv.TestId05 = t5.Id
LEFT JOIN 
    Tests t6 ON ptrv.TestId06 = t6.Id
LEFT JOIN 
    Tests t7 ON ptrv.TestId07 = t7.Id
LEFT JOIN 
    Tests t8 ON ptrv.TestId08 = t8.Id
LEFT JOIN 
    Tests t9 ON ptrv.TestId09 = t9.Id
WHERE
    ptrv.DataFlag = 1 AND
    (ptrv.CreatedDate >= @StartDate OR @StartDate IS NULL) AND
    (ptrv.CreatedDate <= @EndDate OR @EndDate IS NULL) AND
    (ptrv.ProbeSn LIKE CONCAT('%', @ProbeSn, '%') OR @ProbeSn IS NULL OR @ProbeSn = '') AND
    (ptrv.TransducerModuleSn LIKE CONCAT('%', @TransducerModuleSn, '%') OR @TransducerModuleSn IS NULL OR @TransducerModuleSn = '') AND
    (ptrv.TransducerSn LIKE CONCAT('%', @TransducerSn, '%') OR @TransducerSn IS NULL OR @TransducerSn = '') AND
    (ptrv.MotorModuleSn LIKE CONCAT('%', @MotorModuleSn, '%') OR @MotorModuleSn IS NULL OR @MotorModuleSn = '')
";
        public async Task<List<ProbeTestResult>> GetProbeTestResultSqlAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            List<ProbeTestResult> pTRViews = await _context.Set<ProbeTestResult>()
                .FromSqlRaw(PTRViewQuery,
                    new MySqlParameter("@StartDate", startDate),
                    new MySqlParameter("@EndDate", endDate),
                    new MySqlParameter("@ProbeSn", probeSn),
                    new MySqlParameter("@TransducerModuleSn", transducerModuleSn),
                    new MySqlParameter("@TransducerSn", transducerSn),
                    new MySqlParameter("@MotorModuleSn ", motorModuleSn)
                )
                .ToListAsync();
            return pTRViews;
        }

        public async Task<List<ProbeTestResult>> GetProbeTestResultLinqAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            IQueryable<ProbeTestResult> query = 
                from ptrv in _context.Set<PTRView>()
                join t1 in _context.Set<Test>() on ptrv.TestId01 equals t1.Id into t1Group
                from t1 in t1Group.DefaultIfEmpty()
                join t2 in _context.Set<Test>() on ptrv.TestId02 equals t2.Id into t2Group
                from t2 in t2Group.DefaultIfEmpty()
                join t3 in _context.Set<Test>() on ptrv.TestId03 equals t3.Id into t3Group
                from t3 in t3Group.DefaultIfEmpty()
                join t4 in _context.Set<Test>() on ptrv.TestId04 equals t4.Id into t4Group
                from t4 in t4Group.DefaultIfEmpty()
                join t5 in _context.Set<Test>() on ptrv.TestId05 equals t5.Id into t5Group
                from t5 in t5Group.DefaultIfEmpty()
                join t6 in _context.Set<Test>() on ptrv.TestId06 equals t6.Id into t6Group
                from t6 in t6Group.DefaultIfEmpty()
                join t7 in _context.Set<Test>() on ptrv.TestId07 equals t7.Id into t7Group
                from t7 in t7Group.DefaultIfEmpty()
                join t8 in _context.Set<Test>() on ptrv.TestId08 equals t8.Id into t8Group
                from t8 in t8Group.DefaultIfEmpty()
                join t9 in _context.Set<Test>() on ptrv.TestId09 equals t9.Id into t9Group
                from t9 in t9Group.DefaultIfEmpty()
                where 
                    ptrv.DataFlag == 1 &&
                    (startDate == null || ptrv.CreatedDate >= startDate) &&
                    (endDate == null || ptrv.CreatedDate <= endDate) &&
                    (string.IsNullOrEmpty(probeSn) || ptrv.ProbeSn.Contains(probeSn)) &&
                    (string.IsNullOrEmpty(transducerModuleSn) || ptrv.TransducerModuleSn.Contains(transducerModuleSn)) &&
                    (string.IsNullOrEmpty(transducerSn) || ptrv.TransducerSn.Contains(transducerSn)) &&
                    (string.IsNullOrEmpty(motorModuleSn) || ptrv.MotorModuleSn.Contains(motorModuleSn))
                select new ProbeTestResult
                {
                    Id = ptrv.Id,
                    ProbeSn = ptrv.ProbeSn,
                    CreatedDate = ptrv.CreatedDate,
                    TransducerModuleSn = ptrv.TransducerModuleSn,
                    TransducerSn = ptrv.TransducerSn,
                    MotorModuleSn = ptrv.MotorModuleSn,
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

            List<ProbeTestResult> pTRViews = await query.ToListAsync();
            return pTRViews;
        }
    }
}
