using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;
using SonoCap.MES.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace SonoCap.MES.Repositories
{
    public class ProbeRepository : RepositoryBase<Probe>, IProbeRepository
    {
        private string _sql =
            @"SELECT
                p.Id,
                p.ProbeSn,
                p.CreatedDate,
                tm.TransducerModuleSn ,
                tm.TransducerSn ,
                mm.MotorModuleSn ,
                t1.CategoryId AS CategoryId1_1,
                t1.TestTypeId AS TestTypeId1_1,
                t1.CreatedDate AS CreatedDate1_1,
                t1.Result AS Result1_1,
                t2.CategoryId AS CategoryId1_2,
                t2.TestTypeId AS TestTypeId1_2,
                t2.CreatedDate AS CreatedDate1_2,
                t2.Result AS Result1_2,
                t3.CategoryId AS CategoryId1_3,
                t3.TestTypeId AS TestTypeId1_3,
                t3.CreatedDate AS CreatedDate1_3,
                t3.Result AS Result1_3,
                t4.CategoryId AS CategoryId1_4,
                t4.TestTypeId AS TestTypeId1_4,
                t4.CreatedDate AS CreatedDate1_4,
                t4.Result AS Result1_4,
                t5.CategoryId AS CategoryId1_5,
                t5.TestTypeId AS TestTypeId1_5,
                t5.CreatedDate AS CreatedDate1_5,
                t5.Result AS Result1_5,
                t6.CategoryId AS CategoryId1_6,
                t6.TestTypeId AS TestTypeId1_6,
                t6.CreatedDate AS CreatedDate1_6,
                t6.Result AS Result1_6
            FROM
                Probes p
            LEFT JOIN
                TransducerModules tm ON p.TransducerModuleId = tm.Id AND tm.DataFlag = 1
            LEFT JOIN
                MotorModules mm ON p.TransducerModuleId = mm.Id AND mm.DataFlag = 1
            LEFT JOIN
                Tests t1 ON p.TransducerModuleId = t1.TransducerModuleId AND t1.Id = (SELECT MAX(Id) FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 1 AND TestTypeId = 1 AND DataFlag = 1)
            LEFT JOIN
                Tests t2 ON p.TransducerModuleId = t2.TransducerModuleId AND t2.Id = (SELECT MAX(Id) FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 1 AND TestTypeId = 2 AND DataFlag = 1)
            LEFT JOIN
                Tests t3 ON p.TransducerModuleId = t3.TransducerModuleId AND t3.Id = (SELECT MAX(Id) FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 1 AND TestTypeId = 3 AND DataFlag = 1)
            LEFT JOIN
                Tests t4 ON p.TransducerModuleId = t4.TransducerModuleId AND t4.Id = (SELECT MAX(Id) FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 2 AND TestTypeId = 1 AND DataFlag = 1)
            LEFT JOIN
                Tests t5 ON p.TransducerModuleId = t5.TransducerModuleId AND t5.Id = (SELECT MAX(Id) FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 2 AND TestTypeId = 2 AND DataFlag = 1)
            LEFT JOIN
                Tests t6 ON p.TransducerModuleId = t6.TransducerModuleId AND t6.Id = (SELECT MAX(Id) FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 2 AND TestTypeId = 3 AND DataFlag = 1)";

        private string _sql2 =
            @"SELECT 
                p.Id, 
                p.ProbeSn,
                p.CreatedDate,
                tm.TransducerModuleSn ,
                tm.TransducerSn ,
                mm.MotorModuleSn ,
                t1.CategoryId AS CategoryId1_1,
                t1.TestTypeId AS TestTypeId1_1,
                t1.CreatedDate AS CreatedDate1_1,
                t1.Result AS Result1_1,
                t2.CategoryId AS CategoryId1_2,
                t2.TestTypeId AS TestTypeId1_2,
                t2.CreatedDate AS CreatedDate1_2,
                t2.Result AS Result1_2,
                t3.CategoryId AS CategoryId1_3,
                t3.TestTypeId AS TestTypeId1_3,
                t3.CreatedDate AS CreatedDate1_3,
                t3.Result AS Result1_3,
                t4.CategoryId AS CategoryId1_4,
                t4.TestTypeId AS TestTypeId1_4,
                t4.CreatedDate AS CreatedDate1_4,
                t4.Result AS Result1_4,
                t5.CategoryId AS CategoryId1_5,
                t5.TestTypeId AS TestTypeId1_5,
                t5.CreatedDate AS CreatedDate1_5,
                t5.Result AS Result1_5,
                t6.CategoryId AS CategoryId1_6,
                t6.TestTypeId AS TestTypeId1_6,
                t6.CreatedDate AS CreatedDate1_6,
                t6.Result AS Result1_6
            FROM 
                Probes p
            LEFT JOIN 
                TransducerModules tm ON p.TransducerModuleId = tm.Id AND tm.DataFlag = 1
            LEFT JOIN 
                MotorModules mm ON p.TransducerModuleId = mm.Id AND mm.DataFlag = 1    
            LEFT JOIN 
                Tests t1 ON p.TransducerModuleId = t1.TransducerModuleId AND t1.CategoryId = 1 AND t1.TestTypeId = 1 AND t1.DataFlag = 1
            LEFT JOIN 
                Tests t2 ON p.TransducerModuleId = t2.TransducerModuleId AND t2.CategoryId = 1 AND t2.TestTypeId = 2 AND t2.DataFlag = 1
            LEFT JOIN 
                Tests t3 ON p.TransducerModuleId = t3.TransducerModuleId AND t3.CategoryId = 1 AND t3.TestTypeId = 3 AND t3.DataFlag = 1
            LEFT JOIN 
                Tests t4 ON p.TransducerModuleId = t4.TransducerModuleId AND t4.CategoryId = 2 AND t4.TestTypeId = 1 AND t4.DataFlag = 1
            LEFT JOIN 
                Tests t5 ON p.TransducerModuleId = t5.TransducerModuleId AND t5.CategoryId = 2 AND t5.TestTypeId = 2 AND t5.DataFlag = 1
            LEFT JOIN 
                Tests t6 ON p.TransducerModuleId = t6.TransducerModuleId AND t6.CategoryId = 2 AND t6.TestTypeId = 3 AND t6.DataFlag = 1";

        private string _sqlData =
            @"SELECT 
                p.Id, 
                p.ProbeSn,
                p.CreatedDate,
                tm.TransducerModuleSn ,
                tm.TransducerSn ,
                mm.MotorModuleSn ,
                t1.CategoryId AS CategoryId1_1,
                t1.TestTypeId AS TestTypeId1_1,
                t1.CreatedDate AS CreatedDate1_1,
                t1.Result AS Result1_1,
                t2.CategoryId AS CategoryId1_2,
                t2.TestTypeId AS TestTypeId1_2,
                t2.CreatedDate AS CreatedDate1_2,
                t2.Result AS Result1_2,
                t3.CategoryId AS CategoryId1_3,
                t3.TestTypeId AS TestTypeId1_3,
                t3.CreatedDate AS CreatedDate1_3,
                t3.Result AS Result1_3,
                t4.CategoryId AS CategoryId1_4,
                t4.TestTypeId AS TestTypeId1_4,
                t4.CreatedDate AS CreatedDate1_4,
                t4.Result AS Result1_4,
                t5.CategoryId AS CategoryId1_5,
                t5.TestTypeId AS TestTypeId1_5,
                t5.CreatedDate AS CreatedDate1_5,
                t5.Result AS Result1_5,
                t6.CategoryId AS CategoryId1_6,
                t6.TestTypeId AS TestTypeId1_6,
                t6.CreatedDate AS CreatedDate1_6,
                t6.Result AS Result1_6
            FROM 
                Probes p
            LEFT JOIN 
                TransducerModules tm ON p.TransducerModuleId = tm.Id AND tm.DataFlag = 1
            LEFT JOIN 
                MotorModules mm ON p.TransducerModuleId = mm.Id AND mm.DataFlag = 1    
            LEFT JOIN 
                Tests t1 ON p.TransducerModuleId = t1.TransducerModuleId AND t1.Id = (SELECT MAX(Id) FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 1 AND TestTypeId = 1 AND DataFlag = 1)
            LEFT JOIN 
                Tests t2 ON p.TransducerModuleId = t2.TransducerModuleId AND t2.Id = (SELECT MAX(Id) FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 1 AND TestTypeId = 2 AND DataFlag = 1)
            LEFT JOIN 
                Tests t3 ON p.TransducerModuleId = t3.TransducerModuleId AND t3.Id = (SELECT MAX(Id) FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 1 AND TestTypeId = 3 AND DataFlag = 1)
            LEFT JOIN 
                Tests t4 ON p.TransducerModuleId = t4.TransducerModuleId AND t4.Id = (SELECT MAX(Id) FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 2 AND TestTypeId = 1 AND DataFlag = 1)
            LEFT JOIN 
                Tests t5 ON p.TransducerModuleId = t5.TransducerModuleId AND t5.Id = (SELECT MAX(Id) FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 2 AND TestTypeId = 2 AND DataFlag = 1)
            LEFT JOIN 
                Tests t6 ON p.TransducerModuleId = t6.TransducerModuleId AND t6.Id = (SELECT MAX(Id) FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 2 AND TestTypeId = 3 AND DataFlag = 1)
            WHERE
                (p.CreatedDate >= @StartDate OR @StartDate IS NULL) AND
                (p.CreatedDate <= @EndDate OR @EndDate IS NULL) AND
                (p.ProbeSn = @ProbeSn OR @ProbeSn IS NULL) AND
                (tm.TransducerModuleSn = @TransducerModuleSn OR @TransducerModuleSn IS NULL) AND
                (tm.TransducerSn = @TransducerSn OR @TransducerSn IS NULL) AND
                (mm.MotorModuleSn = @MotorModuleSn OR @MotorModuleSn IS NULL)";

        private string _sqlData1 =
            @"SELECT 
            p.Id, 
            p.ProbeSn,
            p.CreatedDate,
            tm.TransducerModuleSn ,
            tm.TransducerSn ,
            mm.MotorModuleSn ,
            t1.CategoryId AS CategoryId1_1,
            t1.TestTypeId AS TestTypeId1_1,
            t1.CreatedDate AS CreatedDate1_1,
            t1.Result AS Result1_1,
            t2.CategoryId AS CategoryId1_2,
            t2.TestTypeId AS TestTypeId1_2,
            t2.CreatedDate AS CreatedDate1_2,
            t2.Result AS Result1_2,
            t3.CategoryId AS CategoryId1_3,
            t3.TestTypeId AS TestTypeId1_3,
            t3.CreatedDate AS CreatedDate1_3,
            t3.Result AS Result1_3,
            t4.CategoryId AS CategoryId1_4,
            t4.TestTypeId AS TestTypeId1_4,
            t4.CreatedDate AS CreatedDate1_4,
            t4.Result AS Result1_4,
            t5.CategoryId AS CategoryId1_5,
            t5.TestTypeId AS TestTypeId1_5,
            t5.CreatedDate AS CreatedDate1_5,
            t5.Result AS Result1_5,
            t6.CategoryId AS CategoryId1_6,
            t6.TestTypeId AS TestTypeId1_6,
            t6.CreatedDate AS CreatedDate1_6,
            t6.Result AS Result1_6
        FROM 
            Probes p
        LEFT JOIN 
            TransducerModules tm ON p.TransducerModuleId = tm.Id AND tm.DataFlag = 1
        LEFT JOIN 
            MotorModules mm ON p.TransducerModuleId = mm.Id AND mm.DataFlag = 1    
        LEFT JOIN 
            Tests t1 ON p.TransducerModuleId = t1.TransducerModuleId AND t1.Id = (SELECT Id FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 1 AND TestTypeId = 1 AND DataFlag = 1 ORDER BY Id DESC LIMIT 1)
        LEFT JOIN 
            Tests t2 ON p.TransducerModuleId = t2.TransducerModuleId AND t2.Id = (SELECT Id FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 1 AND TestTypeId = 2 AND DataFlag = 1 ORDER BY Id DESC LIMIT 1)
        LEFT JOIN 
            Tests t3 ON p.TransducerModuleId = t3.TransducerModuleId AND t3.Id = (SELECT Id FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 1 AND TestTypeId = 3 AND DataFlag = 1 ORDER BY Id DESC LIMIT 1)
        LEFT JOIN 
            Tests t4 ON p.TransducerModuleId = t4.TransducerModuleId AND t4.Id = (SELECT Id FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 2 AND TestTypeId = 1 AND DataFlag = 1 ORDER BY Id DESC LIMIT 1)
        LEFT JOIN 
            Tests t5 ON p.TransducerModuleId = t5.TransducerModuleId AND t5.Id = (SELECT Id FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 2 AND TestTypeId = 2 AND DataFlag = 1 ORDER BY Id DESC LIMIT 1)
        LEFT JOIN 
            Tests t6 ON p.TransducerModuleId = t6.TransducerModuleId AND t6.Id = (SELECT Id FROM Tests WHERE TransducerModuleId = p.TransducerModuleId AND CategoryId = 2 AND TestTypeId = 3 AND DataFlag = 1 ORDER BY Id DESC LIMIT 1)
        WHERE
            (p.CreatedDate >= @StartDate OR @StartDate IS NULL) AND
            (p.CreatedDate <= @EndDate OR @EndDate IS NULL) AND
            (p.ProbeSn = @ProbeSn OR @ProbeSn IS NULL) AND
            (tm.TransducerModuleSn = @TransducerModuleSn OR @TransducerModuleSn IS NULL) AND
            (tm.TransducerSn = @TransducerSn OR @TransducerSn IS NULL) AND
            (mm.MotorModuleSn = @MotorModuleSn OR @MotorModuleSn IS NULL)";

        private string _sqlData2 =
            @"SELECT 
                p.Id, 
                p.ProbeSn,
                p.CreatedDate,
                tm.TransducerModuleSn ,
                tm.TransducerSn ,
                mm.MotorModuleSn ,
                t1.CategoryId AS CategoryId1_1,
                t1.TestTypeId AS TestTypeId1_1,
                t1.CreatedDate AS CreatedDate1_1,
                t1.Result AS Result1_1,
                t2.CategoryId AS CategoryId1_2,
                t2.TestTypeId AS TestTypeId1_2,
                t2.CreatedDate AS CreatedDate1_2,
                t2.Result AS Result1_2,
                t3.CategoryId AS CategoryId1_3,
                t3.TestTypeId AS TestTypeId1_3,
                t3.CreatedDate AS CreatedDate1_3,
                t3.Result AS Result1_3,
                t4.CategoryId AS CategoryId1_4,
                t4.TestTypeId AS TestTypeId1_4,
                t4.CreatedDate AS CreatedDate1_4,
                t4.Result AS Result1_4,
                t5.CategoryId AS CategoryId1_5,
                t5.TestTypeId AS TestTypeId1_5,
                t5.CreatedDate AS CreatedDate1_5,
                t5.Result AS Result1_5,
                t6.CategoryId AS CategoryId1_6,
                t6.TestTypeId AS TestTypeId1_6,
                t6.CreatedDate AS CreatedDate1_6,
                t6.Result AS Result1_6
            FROM 
                Probes p
            LEFT JOIN 
                TransducerModules tm ON p.TransducerModuleId = tm.Id AND tm.DataFlag = 1
            LEFT JOIN 
                MotorModules mm ON p.TransducerModuleId = mm.Id AND mm.DataFlag = 1    
            LEFT JOIN 
                Tests t1 ON p.TransducerModuleId = t1.TransducerModuleId AND t1.CategoryId = 1 AND t1.TestTypeId = 1 AND t1.DataFlag = 1
            LEFT JOIN 
                Tests t2 ON p.TransducerModuleId = t2.TransducerModuleId AND t2.CategoryId = 1 AND t2.TestTypeId = 2 AND t2.DataFlag = 1
            LEFT JOIN 
                Tests t3 ON p.TransducerModuleId = t3.TransducerModuleId AND t3.CategoryId = 1 AND t3.TestTypeId = 3 AND t3.DataFlag = 1
            LEFT JOIN 
                Tests t4 ON p.TransducerModuleId = t4.TransducerModuleId AND t4.CategoryId = 2 AND t4.TestTypeId = 1 AND t4.DataFlag = 1
            LEFT JOIN 
                Tests t5 ON p.TransducerModuleId = t5.TransducerModuleId AND t5.CategoryId = 2 AND t5.TestTypeId = 2 AND t5.DataFlag = 1
            LEFT JOIN 
                Tests t6 ON p.TransducerModuleId = t6.TransducerModuleId AND t6.CategoryId = 2 AND t6.TestTypeId = 3 AND t6.DataFlag = 1
            WHERE 
                p.CreatedDate >= @StartDate AND p.CreatedDate <= @EndDate AND
                (p.ProbeSn = @ProbeSn OR @ProbeSn IS NULL OR @ProbeSn = '') AND
                (tm.TransducerModuleSn = @TransducerModuleSn OR @TransducerModuleSn IS NULL OR @TransducerModuleSn = '') AND
                (tm.TransducerSn = @TransducerSn OR @TransducerSn IS NULL OR @TransducerSn = '') AND
                (mm.MotorModuleSn = @MotorModuleSn OR @MotorModuleSn IS NULL OR @MotorModuleSn = '')";

        public ProbeRepository(MESDbContext context) : base(context)
        {
        }

        public List<ProbeTestResult> GetProbeTestResult()
        {
            // Probe 테이블과 TransducerModule 테이블을 조인
            var query =
                from p in _context.Set<Probe>()
                join tm in _context.Set<TransducerModule>()
                    on p.TransducerModuleId equals tm.Id into tmGroup
                from tm in tmGroup.DefaultIfEmpty()
                join mm in _context.Set<MotorModule>()
                    on p.TransducerModuleId equals mm.Id into mmGroup
                from mm in mmGroup.DefaultIfEmpty()
                select new { Probe = p, TransducerModule = tm, MotorModule = mm };

            // Test 테이블에서 필요한 데이터를 가져오기 위해 분할된 쿼리를 조합
            IQueryable<ProbeTestResult> probeTestResults =
                from q in query
                let t1 = _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 1 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t2 = _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 2 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t3 = _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 3 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t4 = _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 1 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t5 = _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 2 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t6 = _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 3 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                // 나머지 Test들도 동일하게 처리
                select new ProbeTestResult
                {
                    Id = q.Probe.Id,
                    ProbeSN = q.Probe.ProbeSn,
                    CreatedDate = q.Probe.CreatedDate,
                    TransducerModuleSN = q.TransducerModule.TransducerModuleSn,
                    TransducerSN = q.TransducerModule.TransducerSn,
                    MotorModuleSn = q.MotorModule.MotorModuleSn,
                    TestResults = new List<TestResult>
                    {
                        // Test 결과 추가
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t1.CategoryId, TypeId = (Models.Enums.TestTypes)t1.TestTypeId, CreatedDate = t1.CreatedDate, Result = t1.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t2.CategoryId, TypeId = (Models.Enums.TestTypes)t2.TestTypeId, CreatedDate = t2.CreatedDate, Result = t2.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t3.CategoryId, TypeId = (Models.Enums.TestTypes)t3.TestTypeId, CreatedDate = t3.CreatedDate, Result = t3.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t4.CategoryId, TypeId = (Models.Enums.TestTypes)t4.TestTypeId, CreatedDate = t4.CreatedDate, Result = t4.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t5.CategoryId, TypeId = (Models.Enums.TestTypes)t5.TestTypeId, CreatedDate = t5.CreatedDate, Result = t5.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t6.CategoryId, TypeId = (Models.Enums.TestTypes)t6.TestTypeId, CreatedDate = t6.CreatedDate, Result = t6.ChangedImgMetadata},
                    }
                };
            return probeTestResults.ToList();
        }

        public List<ProbeTestResult> GetProbeTestResult1()
        {
            IEnumerable<ProbeTestResult> probeTestResults =
                (from p in _context.Set<Probe>()
                 join tm in _context.Set<TransducerModule>()
                     on p.TransducerModuleId equals tm.Id into tmGroup
                 from tm in tmGroup.DefaultIfEmpty()
                 join mm in _context.Set<MotorModule>()
                     on p.TransducerModuleId equals mm.Id into mmGroup
                 from mm in mmGroup.DefaultIfEmpty()
                 join t1 in _context.Set<Test>()
                     on new { ModuleId = p.TransducerModuleId, CategoryId = 1, TestTypeId = 1 }
                     equals new { ModuleId = t1.TransducerModuleId, t1.CategoryId, t1.TestTypeId } into t1Group
                 from t1 in t1Group.DefaultIfEmpty()
                 join t2 in _context.Set<Test>()
                     on new { ModuleId = p.TransducerModuleId, CategoryId = 1, TestTypeId = 2 }
                     equals new { ModuleId = t2.TransducerModuleId, t2.CategoryId, t2.TestTypeId } into t2Group
                 from t2 in t2Group.DefaultIfEmpty()
                 join t3 in _context.Set<Test>()
                     on new { ModuleId = p.TransducerModuleId, CategoryId = 1, TestTypeId = 3 }
                     equals new { ModuleId = t3.TransducerModuleId, t3.CategoryId, t3.TestTypeId } into t3Group
                 from t3 in t3Group.DefaultIfEmpty()
                 join t4 in _context.Set<Test>()
                     on new { ModuleId = p.TransducerModuleId, CategoryId = 2, TestTypeId = 1 }
                     equals new { ModuleId = t4.TransducerModuleId, t4.CategoryId, t4.TestTypeId } into t4Group
                 from t4 in t4Group.DefaultIfEmpty()
                 join t5 in _context.Set<Test>()
                     on new { ModuleId = p.TransducerModuleId, CategoryId = 2, TestTypeId = 2 }
                     equals new { ModuleId = t5.TransducerModuleId, t5.CategoryId, t5.TestTypeId } into t5Group
                 from t5 in t5Group.DefaultIfEmpty()
                 join t6 in _context.Set<Test>()
                     on new { ModuleId = p.TransducerModuleId, CategoryId = 2, TestTypeId = 3 }
                     equals new { ModuleId = t6.TransducerModuleId, t6.CategoryId, t6.TestTypeId } into t6Group
                 from t6 in t6Group.DefaultIfEmpty()
                 select new ProbeTestResult
                 {
                     Id = p.Id,
                     ProbeSN = p.ProbeSn,
                     CreatedDate = p.CreatedDate,
                     TransducerModuleSN = tm.TransducerModuleSn,
                     TransducerSN = tm.TransducerSn,
                     MotorModuleSn = mm.MotorModuleSn,
                     TestResults = new List<TestResult>
                        {
                            new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t1.CategoryId, TypeId = (Models.Enums.TestTypes)t1.TestTypeId, CreatedDate = t1.CreatedDate, Result = t1.ChangedImgMetadata},
                            new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t2.CategoryId, TypeId = (Models.Enums.TestTypes)t2.TestTypeId, CreatedDate = t2.CreatedDate, Result = t2.ChangedImgMetadata},
                            new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t3.CategoryId, TypeId = (Models.Enums.TestTypes)t3.TestTypeId, CreatedDate = t3.CreatedDate, Result = t3.ChangedImgMetadata},
                            new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t4.CategoryId, TypeId = (Models.Enums.TestTypes)t4.TestTypeId, CreatedDate = t4.CreatedDate, Result = t4.ChangedImgMetadata},
                            new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t5.CategoryId, TypeId = (Models.Enums.TestTypes)t5.TestTypeId, CreatedDate = t5.CreatedDate, Result = t5.ChangedImgMetadata},
                            new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t6.CategoryId, TypeId = (Models.Enums.TestTypes)t6.TestTypeId, CreatedDate = t6.CreatedDate, Result = t6.ChangedImgMetadata},
                        }
                 });
            return probeTestResults.ToList();
        }

        public List<ProbeTestResult> GetProbeTestResult2()
        {
            IEnumerable<ProbeTestResult> probeTestResults =
            (from p in _context.Set<Probe>()
             join tm in _context.Set<TransducerModule>()
                 on p.TransducerModuleId equals tm.Id into tmGroup
             from tm in tmGroup.DefaultIfEmpty()
             join mm in _context.Set<MotorModule>()
                 on p.TransducerModuleId equals mm.Id into mmGroup
             from mm in mmGroup.DefaultIfEmpty()
             join t1 in _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 1 && t.DataFlag == 1)
                 on p.TransducerModuleId equals t1.TransducerModuleId into t1Group
             from t1 in t1Group.OrderByDescending(t => t.Id).Take(1).DefaultIfEmpty()
             join t2 in _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 2 && t.DataFlag == 1)
                 on p.TransducerModuleId equals t2.TransducerModuleId into t2Group
             from t2 in t2Group.OrderByDescending(t => t.Id).Take(1).DefaultIfEmpty()
             join t3 in _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 3 && t.DataFlag == 1)
                 on p.TransducerModuleId equals t3.TransducerModuleId into t3Group
             from t3 in t3Group.OrderByDescending(t => t.Id).Take(1).DefaultIfEmpty()
             join t4 in _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 1 && t.DataFlag == 1)
                 on p.TransducerModuleId equals t4.TransducerModuleId into t4Group
             from t4 in t4Group.OrderByDescending(t => t.Id).Take(1).DefaultIfEmpty()
             join t5 in _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 2 && t.DataFlag == 1)
                 on p.TransducerModuleId equals t5.TransducerModuleId into t5Group
             from t5 in t5Group.OrderByDescending(t => t.Id).Take(1).DefaultIfEmpty()
             join t6 in _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 3 && t.DataFlag == 1)
                 on p.TransducerModuleId equals t6.TransducerModuleId into t6Group
             from t6 in t6Group.OrderByDescending(t => t.Id).Take(1).DefaultIfEmpty()
             select new ProbeTestResult
             {
                 Id = p.Id,
                 ProbeSN = p.ProbeSn,
                 CreatedDate = p.CreatedDate,
                 TransducerModuleSN = tm.TransducerModuleSn,
                 TransducerSN = tm.TransducerSn,
                 MotorModuleSn = mm.MotorModuleSn,
                 TestResults =
                 new List<TestResult>
                    {
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t1.CategoryId, TypeId = (Models.Enums.TestTypes)t1.TestTypeId, CreatedDate = t1.CreatedDate, Result = t1.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t2.CategoryId, TypeId = (Models.Enums.TestTypes)t2.TestTypeId, CreatedDate = t2.CreatedDate, Result = t2.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t3.CategoryId, TypeId = (Models.Enums.TestTypes)t3.TestTypeId, CreatedDate = t3.CreatedDate, Result = t3.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t4.CategoryId, TypeId = (Models.Enums.TestTypes)t4.TestTypeId, CreatedDate = t4.CreatedDate, Result = t4.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t5.CategoryId, TypeId = (Models.Enums.TestTypes)t5.TestTypeId, CreatedDate = t5.CreatedDate, Result = t5.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t6.CategoryId, TypeId = (Models.Enums.TestTypes)t6.TestTypeId, CreatedDate = t6.CreatedDate, Result = t6.ChangedImgMetadata},
                    }
             });

            var a = probeTestResults.ToList();
            return a;
        }

        public async Task<List<ProbeTestResult>> GetProbeTestResultAsync()
        {
            // Probe 테이블과 TransducerModule 테이블을 조인
            var query =
                from p in _context.Set<Probe>()
                join tm in _context.Set<TransducerModule>()
                    on p.TransducerModuleId equals tm.Id into tmGroup
                from tm in tmGroup.DefaultIfEmpty()
                join mm in _context.Set<MotorModule>()
                    on p.TransducerModuleId equals mm.Id into mmGroup
                from mm in mmGroup.DefaultIfEmpty()
                select new { Probe = p, TransducerModule = tm, MotorModule = mm };

            // Test 테이블에서 필요한 데이터를 가져오기 위해 분할된 쿼리를 조합
            IQueryable<ProbeTestResult> probeTestResults =
                from q in query
                let t1 = _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 1 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t2 = _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 2 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t3 = _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 3 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t4 = _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 1 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t5 = _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 2 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t6 = _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 3 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                // 나머지 Test들도 동일하게 처리
                select new ProbeTestResult
                {
                    Id = q.Probe.Id,
                    ProbeSN = q.Probe.ProbeSn,
                    CreatedDate = q.Probe.CreatedDate,
                    TransducerModuleSN = q.TransducerModule.TransducerModuleSn,
                    TransducerSN = q.TransducerModule.TransducerSn,
                    MotorModuleSn = q.MotorModule.MotorModuleSn,
                    TestResults = new List<TestResult>
                    {
                // Test 결과 추가
                new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t1.CategoryId, TypeId = (Models.Enums.TestTypes)t1.TestTypeId, CreatedDate = t1.CreatedDate, Result = t1.ChangedImgMetadata},
                new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t2.CategoryId, TypeId = (Models.Enums.TestTypes)t2.TestTypeId, CreatedDate = t2.CreatedDate, Result = t2.ChangedImgMetadata},
                new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t3.CategoryId, TypeId = (Models.Enums.TestTypes)t3.TestTypeId, CreatedDate = t3.CreatedDate, Result = t3.ChangedImgMetadata},
                new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t4.CategoryId, TypeId = (Models.Enums.TestTypes)t4.TestTypeId, CreatedDate = t4.CreatedDate, Result = t4.ChangedImgMetadata},
                new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t5.CategoryId, TypeId = (Models.Enums.TestTypes)t5.TestTypeId, CreatedDate = t5.CreatedDate, Result = t5.ChangedImgMetadata},
                new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t6.CategoryId, TypeId = (Models.Enums.TestTypes)t6.TestTypeId, CreatedDate = t6.CreatedDate, Result = t6.ChangedImgMetadata},
                    }
                };

            // 결과를 리스트로 변환하여 반환
            return await probeTestResults.ToListAsync();
        }

        public List<ProbeTestResult> GetProbeTestResult(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            // 초기 쿼리 생성
            var query =
                from p in _context.Set<Probe>()
                join tm in _context.Set<TransducerModule>()
                    on p.TransducerModuleId equals tm.Id into tmGroup
                from tm in tmGroup.DefaultIfEmpty()
                join mm in _context.Set<MotorModule>()
                    on p.TransducerModuleId equals mm.Id into mmGroup
                from mm in mmGroup.DefaultIfEmpty()
                select new { Probe = p, TransducerModule = tm, MotorModule = mm };

            // Test 테이블에서 필요한 데이터를 가져오기 위해 분할된 쿼리를 조합
            IQueryable<ProbeTestResult> probeTestResults =
                from q in query
                let t1 = _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 1 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t2 = _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 2 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t3 = _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 3 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t4 = _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 1 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t5 = _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 2 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t6 = _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 3 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                // 나머지 Test들도 동일하게 처리
                select new ProbeTestResult
                {
                    Id = q.Probe.Id,
                    ProbeSN = q.Probe.ProbeSn,
                    CreatedDate = q.Probe.CreatedDate,
                    TransducerModuleSN = q.TransducerModule.TransducerModuleSn,
                    TransducerSN = q.TransducerModule.TransducerSn,
                    MotorModuleSn = q.MotorModule.MotorModuleSn,
                    TestResults = new List<TestResult>
                    {
                        // Test 결과 추가
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t1.CategoryId, TypeId = (Models.Enums.TestTypes)t1.TestTypeId, CreatedDate = t1.CreatedDate, Result = t1.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t2.CategoryId, TypeId = (Models.Enums.TestTypes)t2.TestTypeId, CreatedDate = t2.CreatedDate, Result = t2.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t3.CategoryId, TypeId = (Models.Enums.TestTypes)t3.TestTypeId, CreatedDate = t3.CreatedDate, Result = t3.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t4.CategoryId, TypeId = (Models.Enums.TestTypes)t4.TestTypeId, CreatedDate = t4.CreatedDate, Result = t4.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t5.CategoryId, TypeId = (Models.Enums.TestTypes)t5.TestTypeId, CreatedDate = t5.CreatedDate, Result = t5.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t6.CategoryId, TypeId = (Models.Enums.TestTypes)t6.TestTypeId, CreatedDate = t6.CreatedDate, Result = t6.ChangedImgMetadata},
                    }
                };

            // 필터 조건 추가
            if (startDate != null)
            {
                probeTestResults = probeTestResults.Where(ptr => ptr.CreatedDate >= startDate);
            }
            if (endDate != null)
            {
                probeTestResults = probeTestResults.Where(ptr => ptr.CreatedDate <= endDate);
            }
            if (!string.IsNullOrEmpty(probeSn))
            {
                probeTestResults = probeTestResults.Where(ptr => ptr.ProbeSN == probeSn);
            }

            if (!string.IsNullOrEmpty(transducerModuleSn))
            {
                probeTestResults = probeTestResults.Where(ptr => ptr.TransducerModuleSN == transducerModuleSn);
            }

            if (!string.IsNullOrEmpty(transducerSn))
            {
                probeTestResults = probeTestResults.Where(ptr => ptr.TransducerSN == transducerSn);
            }

            if (!string.IsNullOrEmpty(motorModuleSn))
            {
                probeTestResults = probeTestResults.Where(ptr => ptr.MotorModuleSn == motorModuleSn);
            }

            // 필터를 적용한 후에 ToList 호출하여 쿼리를 실행
            return probeTestResults.ToList();
        }

        public async Task<List<ProbeTestResult>> GetProbeTestResultAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            // 초기 쿼리 생성
            var query =
                from p in _context.Set<Probe>()
                join tm in _context.Set<TransducerModule>()
                    on p.TransducerModuleId equals tm.Id into tmGroup
                from tm in tmGroup.DefaultIfEmpty()
                join mm in _context.Set<MotorModule>()
                    on p.TransducerModuleId equals mm.Id into mmGroup
                from mm in mmGroup.DefaultIfEmpty()
                select new { Probe = p, TransducerModule = tm, MotorModule = mm };

            // Test 테이블에서 필요한 데이터를 가져오기 위해 분할된 쿼리를 조합
            IQueryable<ProbeTestResult> probeTestResults =
                from q in query
                let t1 = _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 1 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t2 = _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 2 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t3 = _context.Set<Test>().Where(t => t.CategoryId == 1 && t.TestTypeId == 3 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t4 = _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 1 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t5 = _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 2 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                let t6 = _context.Set<Test>().Where(t => t.CategoryId == 2 && t.TestTypeId == 3 && t.DataFlag == 1 && t.TransducerModuleId == q.TransducerModule.Id).OrderByDescending(t => t.Id).FirstOrDefault()
                // 나머지 Test들도 동일하게 처리
                select new ProbeTestResult
                {
                    Id = q.Probe.Id,
                    ProbeSN = q.Probe.ProbeSn,
                    CreatedDate = q.Probe.CreatedDate,
                    TransducerModuleSN = q.TransducerModule.TransducerModuleSn,
                    TransducerSN = q.TransducerModule.TransducerSn,
                    MotorModuleSn = q.MotorModule.MotorModuleSn,
                    TestResults = new List<TestResult>
                    {
                        // Test 결과 추가
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t1.CategoryId, TypeId = (Models.Enums.TestTypes)t1.TestTypeId, CreatedDate = t1.CreatedDate, Result = t1.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t2.CategoryId, TypeId = (Models.Enums.TestTypes)t2.TestTypeId, CreatedDate = t2.CreatedDate, Result = t2.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t3.CategoryId, TypeId = (Models.Enums.TestTypes)t3.TestTypeId, CreatedDate = t3.CreatedDate, Result = t3.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t4.CategoryId, TypeId = (Models.Enums.TestTypes)t4.TestTypeId, CreatedDate = t4.CreatedDate, Result = t4.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t5.CategoryId, TypeId = (Models.Enums.TestTypes)t5.TestTypeId, CreatedDate = t5.CreatedDate, Result = t5.ChangedImgMetadata},
                        new TestResult { CategoryId = (SonoCap.MES.Models.Enums.TestCategories)t6.CategoryId, TypeId = (Models.Enums.TestTypes)t6.TestTypeId, CreatedDate = t6.CreatedDate, Result = t6.ChangedImgMetadata},
                    }
                };

            // 필터 조건 추가
            if (startDate != null)
            {
                probeTestResults = probeTestResults.Where(ptr => ptr.CreatedDate >= startDate);
            }
            if (endDate != null)
            {
                probeTestResults = probeTestResults.Where(ptr => ptr.CreatedDate <= endDate);
            }
            if (!string.IsNullOrEmpty(probeSn))
            {
                probeTestResults = probeTestResults.Where(ptr => ptr.ProbeSN.Contains(probeSn));
            }

            if (!string.IsNullOrEmpty(transducerModuleSn))
            {
                probeTestResults = probeTestResults.Where(ptr => ptr.TransducerModuleSN.Contains(transducerModuleSn));
            }

            if (!string.IsNullOrEmpty(transducerSn))
            {
                probeTestResults = probeTestResults.Where(ptr => ptr.TransducerSN.Contains(transducerSn));
            }

            if (!string.IsNullOrEmpty(motorModuleSn))
            {
                probeTestResults = probeTestResults.Where(ptr => ptr.MotorModuleSn.Contains(motorModuleSn));
            }

            return await probeTestResults.ToListAsync();
        }

        public List<ProbeTestResult> GetProbeTestResultSql()
        {
            List<ProbeTestResult> probeTestResults = _context.Set<ProbeTestResult>().FromSqlRaw(_sql).ToList();
            return probeTestResults;
        }

        public async Task<List<ProbeTestResult>> GetProbeTestResultSqlAsync()
        {
            List<ProbeTestResult> probeTestResults = await _context.Set<ProbeTestResult>().FromSqlRaw(_sql).ToListAsync();
            return probeTestResults;
        }

        public List<ProbeTestResult> GetProbeTestResultSql(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            List<ProbeTestResult> probeTestResults = _context.Set<ProbeTestResult>()
                .FromSqlRaw(_sqlData2,
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate),
                    new SqlParameter("@ProbeSn", probeSn),
                    new SqlParameter("@TransducerModuleSn", transducerModuleSn),
                    new SqlParameter("@TransducerSn", transducerSn),
                    new SqlParameter("@MotorModuleSn ", motorModuleSn)
                )
                .ToList();
            return probeTestResults;
        }

        public async Task<List<ProbeTestResult>> GetProbeTestResultSqlAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn)
        {
            List<ProbeTestResult> probeTestResults = await _context.Set<ProbeTestResult>()
                .FromSqlRaw(_sqlData2,
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate),
                    new SqlParameter("@ProbeSn", probeSn),
                    new SqlParameter("@TransducerModuleSn", transducerModuleSn),
                    new SqlParameter("@TransducerSn", transducerSn),
                    new SqlParameter("@MotorModuleSn ", motorModuleSn)
                )
                .ToListAsync();
            return probeTestResults;
        }
    }
}
