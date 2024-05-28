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
        public PTRViewRepository(MESDbContext context) : base(context)
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
    }
}
