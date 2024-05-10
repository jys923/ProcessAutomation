//#define INSERT_MASTER_2
//#define INSERT_DATA_REPO_3
//#define INSERT_MASTER_REPO_3
//#define INSERT_DATA_3
//#define SELECT_SQL_3

//#define INSERT_DATA_REPO_2
//#define INSERT_MASTER_REPO_2
//#define SELECT_SQL_4

using EFCoreSample.Data;
using EFCoreSample.Entities;
using EFCoreSample.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using TestCategory = EFCoreSample.Entities.TestCategory;
using TestType = EFCoreSample.Entities.TestType;
using TransducerModule = EFCoreSample.Entities.TransducerModule;

public class Program
{
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // 서비스 등록
        services.AddTransient<ITesterTypeRepository, TesterTypeRepository>();
        services.AddTransient<ITestRepository, TestRepository>();
        services.AddTransient<ITestTypeRepository, TestTypeRepository>();
        services.AddTransient<IProbeSNRepository, ProbeSNRepository>();
        services.AddTransient<IProbeTypeRepository, ProbeTypeRepository>();

        return services.BuildServiceProvider();
    }

    private static void Main(string[] args)
    {
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        logger.Information("Hello, world!");

        int maxCnt = 100000 - 1;
        int resultCnt = 5;

        string currentDate = DateTime.Now.ToString("yyMMdd");

        Random random = new Random();
        using (var context = new EFCoreSampleDbContext())
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

#if INSERT_MASTER
            #region 기본값 삽입
            db.ProbeTypes.Add(new ProbeType { Code = "SCP01", Type = "5MHz" });
            db.ProbeTypes.Add(new ProbeType { Code = "SCP02", Type = "7.5MHz" });
            db.ProbeTypes.Add(new ProbeType { Code = "SCP03", Type = "10MHz" });
            db.ProbeSNTypes.Add(new ProbeSNType { DateTime = DateTime.Now.ToString("yyyyMMdd"), PcNo = 1 });
            db.ProbeSNTypes.Add(new ProbeSNType { DateTime = DateTime.Now.ToString("yyyyMMdd"), PcNo = 2 });
            db.ProbeSNTypes.Add(new ProbeSNType { DateTime = DateTime.Now.ToString("yyyyMMdd"), PcNo = 3 });
            db.ProbeSNTypes.Add(new ProbeSNType { DateTime = DateTime.Now.ToString("yyyyMMdd"), PcNo = 4 });
            db.TestTypes.Add(new TestType { Name = "1st test", Detail = "align" });
            db.TestTypes.Add(new TestType { Name = "2nd test", Detail = "center" });
            db.TestTypes.Add(new TestType { Name = "3rd test", Detail = "circle" });
            db.TestTypes.Add(new TestType { Name = "4th test", Detail = "red" });
            db.TestTypes.Add(new TestType { Name = "5th test", Detail = "green" });
            db.TesterTypes.Add(new TesterType { Name = "yoon", PcNo = 1 });
            db.TesterTypes.Add(new TesterType { Name = "sang", PcNo = 2 });
            db.TesterTypes.Add(new TesterType { Name = "ko", PcNo = 3 });
            db.TesterTypes.Add(new TesterType { Name = "kwon", PcNo = 4 });
            db.SaveChanges();
            #endregion
#endif

#if INSERT_MASTER_2
            #region 기본값 삽입 
            context.TransducerTypes.Add(new TransducerType { Code = "SCP01", Type = "5.0MHz" });
            context.TransducerTypes.Add(new TransducerType { Code = "SCP02", Type = "7.5MHz" });
            context.TransducerTypes.Add(new TransducerType { Code = "SCP03", Type = "10MHz" });
            context.TestTypes.Add(new TestType { Name = "1st align", Detail = "align" });
            context.TestTypes.Add(new TestType { Name = "2nd center", Detail = "center" });
            context.TestTypes.Add(new TestType { Name = "3rd circle", Detail = "circle" });
            context.Testers.Add(new Tester { Name = "yoon", PcNo = 1 });
            context.Testers.Add(new Tester { Name = "sang", PcNo = 2 });
            context.Testers.Add(new Tester { Name = "ko", PcNo = 3 });
            context.Testers.Add(new Tester { Name = "kwon", PcNo = 4 });
            context.SaveChanges();
            #endregion
#endif

#if INSERT_DATA
            #region probeSN
            for (int i = 0; i < maxCnt; i++)
            {
                db.ProbeSNs.Add(new ProbeSN { ProbeTypeId = random.Next(1, 4), ProbeSNTypeId = random.Next(1, 5), ProbeSeqNo = i + 1 });
            }
            db.SaveChanges();
            #endregion
            
            #region Test
            for (int i = 0; i < maxCnt; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var Test = new Test
                    {
                        TestTypeId = j + 1,
                        TesterTypeId = random.Next(1, 5),
                        OriginalImg = "/img/" + DateTime.Now.ToString("yyyyMMdd") + "/OImg.png",
                        ChangedImg = "/img/" + DateTime.Now.ToString("yyyyMMdd") + "/CImg.png",
                        ChangedImgMetadata = "red circle",
                        Result = random.Next(0, 2),
                        //ProbeSNId = i + 1,
                    };
                    Test.ProbeSNId = i + 1;
                    db.Tests.Add(Test);
                }
            }
            db.SaveChanges();
            #endregion
#endif

#if INSERT_DATA_2
            #region probeSN
            for (int i = 0; i < maxCnt; i++)
            {
                int tmp = random.Next(1, 4);
                string type = Enum.GetName(typeof(Enums.ProbeType), tmp) ?? "SCP01";
                string ProbeSn = type + currentDate + random.Next(1, 4).ToString("D3") + (i + 1).ToString("D5");
                db.ProbeSNs.Add(new ProbeSN { ProbeTypeId = tmp, ProbeSn = ProbeSn });
            }
            db.SaveChanges();
            #endregion
            
            #region Test
            for (int i = 0; i < maxCnt; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var Test = new Test
                    {
                        TestTypeId = j + 1,
                        TesterTypeId = random.Next(1, 5),
                        OriginalImg = $"/img/{currentDate}/{MKRandom(10)}",
                        ChangedImg = $"/img/{currentDate}/{MKRandom(10)}",
                        ChangedImgMetadata = MKSHA256(),
                        Result = random.Next(0, 2),
                        //ProbeSNId = i + 1,
                    };
                    Test.ProbeSNId = i + 1;
                    db.Tests.Add(Test);
                }
            }
            db.SaveChanges();
            #endregion
#endif

#if false
            #region 기본 LINQ
            //List<Test> list = db.Tests.ToList();
            DbSet<Test> Tests = db.Tests;
            List<Test> list = Tests.ToList();
            IEnumerable<Test> enums = Tests.AsEnumerable();
            #endregion
#endif

#if false
            #region 외래키 null 나올떄 대처
            List<Test> list = db.Tests
            .Include(u => u.TestType)
            .Include(u => u.TesterType)
            .Include(u => u.ProbeSN)
            .Include(u => u.ProbeSN.ProbeType)
            .Include(u => u.ProbeSN.ProbeSNType)
            .ToList();
            foreach (var Test in list)
            {
                //Console.WriteLine($"{Test.ProbeSN}.{Test.Result1}.{Test.Result2}.{Test.Result3}.{Test.Result4}.{Test.Result5}");
                Console.WriteLine(Test.ToString());
            }
            #endregion
#endif

#if false
            #region sort get Last
            var query3 = (from main in db.Tests
                          group main by main.ProbeSNId into g
                          select new
                          {
                              ProbeSNId = g.Key,
                              Result1 = (from sub1 in g
                                         where sub1.TestTypeId == 1
                                         orderby sub1.Id descending
                                         select sub1.Result).FirstOrDefault(),
                              Result2 = (from sub2 in g
                                         where sub2.TestTypeId == 2
                                         orderby sub2.Id descending
                                         select sub2.Result).FirstOrDefault(),
                              Result3 = (from sub3 in g
                                         where sub3.TestTypeId == 3
                                         orderby sub3.Id descending
                                         select sub3.Result).FirstOrDefault(),
                              Result4 = (from sub4 in g
                                         where sub4.TestTypeId == 4
                                         orderby sub4.Id descending
                                         select sub4.Result).FirstOrDefault(),
                              Result5 = (from sub5 in g
                                         where sub5.TestTypeId == 5
                                         orderby sub5.Id descending
                                         select sub5.Result).FirstOrDefault()
                          }).ToList();
            #endregion
#endif

#if false
            #region linQ join err why?
            var query2 = from main in db.Tests
                 join ps in db.ProbeSNs on main.ProbeSNId equals ps.Id
                 join pst in db.ProbeSNTypes on ps.ProbeSNTypeId equals pst.Id
                 join pt in db.ProbeTypes on ps.ProbeTypeId equals pt.Id
                 group main by main.ProbeSNId into g
                 select new
                 {
                     ProbeSN = string.Concat(pt.Code, pst.DateTime,
                                              g.Key.ToString().PadLeft(3, '0'),
                                              ps.ProbeSeqNo.ToString().PadLeft(5, '0')),
                     Result1 = (from i in g where i.TestTypeId == 1 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                     Result2 = (from i in g where i.TestTypeId == 2 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                     Result3 = (from i in g where i.TestTypeId == 3 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                     Result4 = (from i in g where i.TestTypeId == 4 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                     Result5 = (from i in g where i.TestTypeId == 5 && i.DataFlag == 1 select i.Result).FirstOrDefault()
                 }; 
    #endregion
#endif

#if SELECT_LINQ
            #region select linq join 30s
            var query = from main in db.Tests
                        join ps in db.ProbeSNs on main.ProbeSNId equals ps.Id
                        join pst in db.ProbeSNTypes on ps.ProbeSNTypeId equals pst.Id
                        join pt in db.ProbeTypes on ps.ProbeTypeId equals pt.Id
                        group main by main.ProbeSNId into g
                        select new
                        {
                            ProbeSN = string.Concat(g.First().ProbeSN.ProbeType.Code, g.First().ProbeSN.ProbeSNType.DateTime,
                                                     g.First().ProbeSN.ProbeSNType.PcNo.ToString().PadLeft(3, '0'),
                                                     g.First().ProbeSN.ProbeSeqNo.ToString().PadLeft(5, '0')),
                            Result1 = (from i in g where i.TestTypeId == 1 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                            Result2 = (from i in g where i.TestTypeId == 2 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                            Result3 = (from i in g where i.TestTypeId == 3 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                            Result4 = (from i in g where i.TestTypeId == 4 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                            Result5 = (from i in g where i.TestTypeId == 5 && i.DataFlag == 1 select i.Result).FirstOrDefault()
                        };

            foreach (var Test in query)
            {
                Console.WriteLine($"{Test.ProbeSN}.{Test.Result1}.{Test.Result2}.{Test.Result3}.{Test.Result4}.{Test.Result5}");
            }
            Console.WriteLine("query.Count : " + query.Count());
            #endregion
#endif

#if SELECT_LINQ_2
            #region select linq join 
            var query = from main in (from i in db.Tests
                                      where i.DataFlag == 1
                                      select i.ProbeSNId).Distinct()
                        join ps in db.ProbeSNs on main equals ps.Id
                        join pt in db.ProbeTypes on ps.ProbeTypeId equals pt.Id
                        join i in db.Tests on main equals i.ProbeSNId
                        where i.DataFlag == 1
                        group i by ps.ProbeSn into g
                        select new
                        {
                            ProbeSN = g.Key,
                            Result1 = g.Where(x => x.TestTypeId == 1).Max(x => x.Result),
                            Result2 = g.Where(x => x.TestTypeId == 2).Max(x => x.Result),
                            Result3 = g.Where(x => x.TestTypeId == 3).Max(x => x.Result),
                            Result4 = g.Where(x => x.TestTypeId == 4).Max(x => x.Result),
                            Result5 = g.Where(x => x.TestTypeId == 5).Max(x => x.Result)
                        };

            foreach (var Test in query)
            {
                Console.WriteLine($"{Test.ProbeSN}.{Test.Result1}.{Test.Result2}.{Test.Result3}.{Test.Result4}.{Test.Result5}");
            }
            Console.WriteLine("query.Count : " + query.Count());
            #endregion
#endif

#if SELECT_SQL
            #region select SQL 8s
            string sql = @"
                        SELECT CONCAT(pt.Code, pst.DateTime, LPAD(pst.PcNo, 3, '0'), LPAD(ps.ProbeSeqNo,5,'0')) AS ProbeSN,
                               z
                               (SELECT Result FROM Tests WHERE ProbeSNId = main.ProbeSNId AND TestTypeId = 2 AND DataFlag = 1) AS Result2,
                               (SELECT Result FROM Tests WHERE ProbeSNId = main.ProbeSNId AND TestTypeId = 3 AND DataFlag = 1) AS Result3,
                               (SELECT Result FROM Tests WHERE ProbeSNId = main.ProbeSNId AND TestTypeId = 4 AND DataFlag = 1) AS Result4,
                               (SELECT Result FROM Tests WHERE ProbeSNId = main.ProbeSNId AND TestTypeId = 5 AND DataFlag = 1) AS Result5
                        FROM (SELECT DISTINCT ProbeSNId FROM Tests) AS main
                        JOIN ProbeSNs ps ON main.ProbeSNId = ps.Id
                        JOIN ProbeSNTypes pst ON ps.ProbeSNTypeId = pst.Id
                        JOIN ProbeTypes pt ON ps.ProbeTypeId = pt.Id";
            var ProbeResults = context.Probes.FromSqlRaw(sql);

            foreach (var probe in ProbeResults)
            {
                Console.WriteLine($"{probe.ProbeSN}.{probe.Result1}.{probe.Result2}.{probe.Result3}.{probe.Result4}.{probe.Result5}");
            }
            Console.WriteLine("ProbeResult.Count : " + ProbeResults.Count());
            #endregion
#elif SELECT_SQL_DYNAMIC
            #region select sql dynamic 18s
            // SQL 쿼리 문자열을 생성
            StringBuilder sqlBuilder = new StringBuilder();
            for (int i = 1; i <= resultCnt; i++)
            {
                sqlBuilder.AppendLine($"(SELECT Result FROM Tests WHERE ProbeSNId = main.ProbeSNId AND TestTypeId = {i} AND DataFlag = 1) AS Result{i},");
            }
            // 마지막 줄에는 쉼표를 제거
            sqlBuilder.Remove(sqlBuilder.Length - 3, 1);

            string sql = $@"
            SELECT CONCAT(pt.Code, pst.DateTime, LPAD(pst.PcNo, 3, '0'), LPAD(ps.ProbeSeqNo,5,'0')) AS ProbeSN,
                   {sqlBuilder}
                FROM (SELECT DISTINCT ProbeSNId FROM Tests) AS main
                JOIN ProbeSNs ps ON main.ProbeSNId = ps.Id
                JOIN ProbeSNTypes pst ON ps.ProbeSNTypeId = pst.Id
                JOIN ProbeTypes pt ON ps.ProbeTypeId = pt.Id";

            List<ProbeSNView> probeSNViews = new List<ProbeSNView>();

            // 데이터베이스 연결 및 쿼리 실행
            using (var command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                db.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        ProbeSNView probeSNView = new ProbeSNView
                        {
                            ProbeSN = result.GetString(0), // 첫 번째 컬럼은 ProbeSN
                            Result = new List<int>()
                        };
                        for (int i = 1; i <= resultCnt; i++)
                        {
                            // Result1, Result2, ... 컬럼은 1부터 시작
                            int? value = result.IsDBNull(i) ? null : (int?)result.GetInt32(i);
                            probeSNView.Result.Add(value ?? 0); // null일 경우 0으로 대체
                        }
                        probeSNViews.Add(probeSNView);
                    }
                }
            }

            /*foreach (var item in probeSNViews)
            {
                Console.Write(item.ProbeSN + " ");
                foreach (var result in item.Result)
                {
                    Console.Write(result + ": ");
                }
                Console.WriteLine("");
            }*/
            Console.WriteLine("probeSNViews.Count : " + probeSNViews.Count());
            #endregion

#elif SELECT_SQL_DYNAMIC_2
            #region select sql dynamic 개선?
            // SQL 쿼리 문자열을 생성
            StringBuilder sqlBuilder = new StringBuilder();
            for (int i = 1; i <= resultCnt; i++)
            {
                sqlBuilder.AppendLine($" MAX(CASE WHEN i.TestTypeId = {i} THEN i.Result END) AS Result{i},");
            }
            // 마지막 줄에는 쉼표를 제거
            sqlBuilder.Remove(sqlBuilder.Length - 3, 1);

            string sql = $@"
            SELECT 
                ps.ProbeSn AS ProbeSN,
                {sqlBuilder}
            FROM 
                (
                    SELECT DISTINCT ProbeSNId 
                    FROM Tests
                    WHERE DataFlag = 1
                ) AS main
            JOIN ProbeSNs ps ON main.ProbeSNId = ps.Id
            JOIN ProbeTypes pt ON ps.ProbeTypeId = pt.Id
            JOIN Tests i ON main.ProbeSNId = i.ProbeSNId AND i.DataFlag = 1
            GROUP BY 
                ps.ProbeSn";

            List<ProbeSNView> probeSNViews = new List<ProbeSNView>();

            // 데이터베이스 연결 및 쿼리 실행
            using (var command = db.Database.GetDbConnection().CreateCommand())
            {command.CommandText = sql;
                db.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        ProbeSNView probeSNView = new ProbeSNView
                        {
                            ProbeSN = result.GetString(0), // 첫 번째 컬럼은 ProbeSN
                            Results = new List<int>()
                        };
                        for (int i = 1; i <= resultCnt; i++)
                        {
                            // Result1, Result2, ... 컬럼은 1부터 시작
                            int? value = result.IsDBNull(i) ? null : (int?)result.GetInt32(i);
                            probeSNView.Results.Add(value ?? 0); // null일 경우 0으로 대체
                        }
                        probeSNViews.Add(probeSNView);
                    }
                }
            }

            /*foreach (var item in probeSNViews)
            {Console.Write(item.ProbeSN + " ");
                foreach (var result in item.Result)
                {
                    Console.Write(result + ": ");
                }
                Console.WriteLine("");
            }*/
            Console.WriteLine("probeSNViews.Count : " + probeSNViews.Count());
            #endregion

#endif
#if INSERT_MASTER_REPO
            #region 기본값 삽입 
            context.ProbeTypes.Add(new ProbeType { Code = "SCP01", Type = "5MHz" });
            context.ProbeTypes.Add(new ProbeType { Code = "SCP02", Type = "7.5MHz" });
            context.ProbeTypes.Add(new ProbeType { Code = "SCP03", Type = "10MHz" });
            context.TestTypes.Add(new TestType { Name = "1st test", Detail = "align" });
            context.TestTypes.Add(new TestType { Name = "2nd test", Detail = "center" });
            context.TestTypes.Add(new TestType { Name = "3rd test", Detail = "circle" });
            context.TestTypes.Add(new TestType { Name = "4th test", Detail = "red" });
            context.TestTypes.Add(new TestType { Name = "5th test", Detail = "green" });
            context.TesterTypes.Add(new TesterType { Name = "yoon", PcNo = 1 });
            context.TesterTypes.Add(new TesterType { Name = "sang", PcNo = 2 });
            context.TesterTypes.Add(new TesterType { Name = "ko", PcNo = 3 });
            context.TesterTypes.Add(new TesterType { Name = "kwon", PcNo = 4 });

            var probeTypeRepository = new ProbeTypeRepository(context);

            context.SaveChanges();
            #endregion
#endif
#if INSERT_DATA_REPO
            #region probeSN
            for (int i = 0; i < maxCnt; i++)
            {
                int tmp = random.Next(1, 4);
                string type = Enum.GetName(typeof(Enums.ProbeType), tmp) ?? "SCP01";
                string ProbeSn = type + currentDate + random.Next(1, 4).ToString("D3") + (i + 1).ToString("D5");
                context.ProbeSNs.Add(new ProbeSN { ProbeTypeId = tmp, ProbeSn = ProbeSn });
            }
            context.SaveChanges();
            #endregion
            
            #region Test
            for (int i = 0; i < maxCnt; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var Test = new Test
                    {
                        TestTypeId = j + 1,
                        TesterTypeId = random.Next(1, 5),
                        OriginalImg = $"/img/{currentDate}/{MKRandom(10)}",
                        ChangedImg = $"/img/{currentDate}/{MKRandom(10)}",
                        ChangedImgMetadata = MKSHA256(),
                        Result = random.Next(0, 2),
                        //ProbeSNId = i + 1,
                    };
                    Test.ProbeSNId = i + 1;
                    context.Tests.Add(Test);
                }
            }
            context.SaveChanges();
            #endregion
#endif

#if INSERT_MASTER_REPO_2
            #region 기본값 삽입 
            context.TransducerTypes.Add(new TransducerType { Code = "SCP01", Type = "5MHz" });
            context.TransducerTypes.Add(new TransducerType { Code = "SCP02", Type = "7.55MHz" });
            context.TransducerTypes.Add(new TransducerType { Code = "SCP03", Type = "10MHz" });

            context.TestCategories.Add(new TestCategory { Name = Enums.TestCategory.Processing.ToString() });
            context.TestCategories.Add(new TestCategory { Name = Enums.TestCategory.Process.ToString() });
            context.TestCategories.Add(new TestCategory { Name = Enums.TestCategory.Dispatch.ToString() });

            context.TestTypes.Add(new TestType { Name = "Align", Detail = "1st test" });
            context.TestTypes.Add(new TestType { Name = "Axial", Detail = "2nd test" });
            context.TestTypes.Add(new TestType { Name = "Lateral", Detail = "3rd test" });

            context.Testers.Add(new Tester { Name = "yoon", PcNo = 1 });
            context.Testers.Add(new Tester { Name = "sang", PcNo = 2 });
            context.Testers.Add(new Tester { Name = "ko", PcNo = 3 });
            context.Testers.Add(new Tester { Name = "kwon", PcNo = 4 });

            context.SaveChanges();
            #endregion
#endif

#if INSERT_DATA_REPO_2
            #region TransducerModules
            for (int i = 0; i < maxCnt; i++)
            {
                int tmp = random.Next(1, 4);
                //string type = Enum.GetName(typeof(Enums.ProbeType), tmp) ?? "SCP01";
                //string type = "SC0P1";
                string TransducerSn = "td-sn " + currentDate + " " + (i + 1).ToString("D3");
                string TransducerModuleSn = "tdm-sn " + currentDate + " " + (i + 1).ToString("D3");
                context.TransducerModules.Add(new TransducerModule { TransducerSn = TransducerSn, TransducerTypeId = tmp, TransducerModuleSn = TransducerModuleSn });
            }
            context.SaveChanges();
            #endregion

            #region MotorModules
            for (int i = 0; i < maxCnt; i++)
            {
                int tmp = random.Next(1, 4);
                //string type = Enum.GetName(typeof(Enums.ProbeType), tmp) ?? "SCP01";
                //string type = "SC0P1";
                string MotorModuleSn = "mtm-sn " + currentDate + " " + (i + 1).ToString("D3");
                context.MotorModules.Add(new MotorModule { MotorModuleSn = MotorModuleSn });
            }
            context.SaveChanges();
            #endregion

            #region Tests
            for (int i = 0; i < maxCnt; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        int randomValue = random.Next(60, 100);
                        int result = randomValue < 65 ? 0 : randomValue;
                        //int result = random.Next(60, 100) < 80 ? 0 : random.Next(60, 100);
                        Test Test = new Test
                        {
                            CategoryId = j + 1,
                            TestTypeId = k + 1,
                            TesterId = random.Next(1, 5),
                            OriginalImg = $"/img/{currentDate}/{MKRandom(10)}",
                            ChangedImg = $"/img/{currentDate}/{MKRandom(10)}",
                            ChangedImgMetadata = MKSHA256(),
                            Result = result,
                            Method = random.Next(0, 2),
                        };
                        Test.TransducerModuleId = i + 1;
                        context.Tests.Add(Test);
                    }
                }
            }
            context.SaveChanges();
            #endregion

            #region Probes
            for (int i = 0; i < maxCnt; i++)
            {
                string ProbeSn = "SCGP01" + currentDate + " " + (i + 1).ToString("D3");
                context.Probes.Add(new Probe { ProbeSn = ProbeSn, TransducerModuleId = i + 1, MotorModuleId = i + 1 });
            }
            context.SaveChanges();
            #endregion

#endif
#if SELECT_SQL_3
            #region SELECT_SQL_3
            string sql = @"
            SELECT 
                p.Id, 
                p.ProbeSn,
                tdm.TransducerModuleSn,
                mtm.MotorModuleSn,
                (SELECT t1_1.CategoryId FROM Tests t1_1 WHERE t1_1.TransducerModuleId = p.TransducerModuleId AND t1_1.CategoryId = 1 AND t1_1.TestTypeId = 1 AND t1_1.DataFlag = 1 LIMIT 1) AS CategoryId1_1,
                (SELECT t1_1.TestTypeId FROM Tests t1_1 WHERE t1_1.TransducerModuleId = p.TransducerModuleId AND t1_1.CategoryId = 1 AND t1_1.TestTypeId = 1 AND t1_1.DataFlag = 1 LIMIT 1) AS TestTypeId1_1,
                (SELECT t1_1.CreatedDate FROM Tests t1_1 WHERE t1_1.TransducerModuleId = p.TransducerModuleId AND t1_1.CategoryId = 1 AND t1_1.TestTypeId = 1 AND t1_1.DataFlag = 1 LIMIT 1) AS CreatedDate1_1,
                (SELECT t1_1.`Result` FROM Tests t1_1 WHERE t1_1.TransducerModuleId = p.TransducerModuleId AND t1_1.CategoryId = 1 AND t1_1.TestTypeId = 1 AND t1_1.DataFlag = 1 LIMIT 1) AS Result1_1,
                (SELECT t1_2.CategoryId FROM Tests t1_2 WHERE t1_2.TransducerModuleId = p.TransducerModuleId AND t1_2.CategoryId = 1 AND t1_2.TestTypeId = 2 AND t1_2.DataFlag = 1 LIMIT 1) AS CategoryId1_2,
                (SELECT t1_2.TestTypeId FROM Tests t1_2 WHERE t1_2.TransducerModuleId = p.TransducerModuleId AND t1_2.CategoryId = 1 AND t1_2.TestTypeId = 2 AND t1_2.DataFlag = 1 LIMIT 1) AS TestTypeId1_2,
                (SELECT t1_2.CreatedDate FROM Tests t1_2 WHERE t1_2.TransducerModuleId = p.TransducerModuleId AND t1_2.CategoryId = 1 AND t1_2.TestTypeId = 2 AND t1_2.DataFlag = 1 LIMIT 1) AS CreatedDate1_2,
                (SELECT t1_2.`Result` FROM Tests t1_2 WHERE t1_2.TransducerModuleId = p.TransducerModuleId AND t1_2.CategoryId = 1 AND t1_2.TestTypeId = 2 AND t1_2.DataFlag = 1 LIMIT 1) AS Result1_2,
                (SELECT t1_3.CategoryId FROM Tests t1_3 WHERE t1_3.TransducerModuleId = p.TransducerModuleId AND t1_3.CategoryId = 1 AND t1_3.TestTypeId = 3 AND t1_3.DataFlag = 1 LIMIT 1) AS CategoryId1_3,
                (SELECT t1_3.TestTypeId FROM Tests t1_3 WHERE t1_3.TransducerModuleId = p.TransducerModuleId AND t1_3.CategoryId = 1 AND t1_3.TestTypeId = 3 AND t1_3.DataFlag = 1 LIMIT 1) AS TestTypeId1_3,
                (SELECT t1_3.CreatedDate FROM Tests t1_3 WHERE t1_3.TransducerModuleId = p.TransducerModuleId AND t1_3.CategoryId = 1 AND t1_3.TestTypeId = 3 AND t1_3.DataFlag = 1 LIMIT 1) AS CreatedDate1_3,
                (SELECT t1_3.`Result` FROM Tests t1_3 WHERE t1_3.TransducerModuleId = p.TransducerModuleId AND t1_3.CategoryId = 1 AND t1_3.TestTypeId = 3 AND t1_3.DataFlag = 1 LIMIT 1) AS Result1_3,
                (SELECT t1_4.CategoryId FROM Tests t1_4 WHERE t1_4.TransducerModuleId = p.TransducerModuleId AND t1_4.CategoryId = 1 AND t1_4.TestTypeId = 4 AND t1_4.DataFlag = 1 LIMIT 1) AS CategoryId1_4,
                (SELECT t1_4.TestTypeId FROM Tests t1_4 WHERE t1_4.TransducerModuleId = p.TransducerModuleId AND t1_4.CategoryId = 1 AND t1_4.TestTypeId = 4 AND t1_4.DataFlag = 1 LIMIT 1) AS TestTypeId1_4,
                (SELECT t1_4.CreatedDate FROM Tests t1_4 WHERE t1_4.TransducerModuleId = p.TransducerModuleId AND t1_4.CategoryId = 1 AND t1_4.TestTypeId = 4 AND t1_4.DataFlag = 1 LIMIT 1) AS CreatedDate1_4,
                (SELECT t1_4.`Result` FROM Tests t1_4 WHERE t1_4.TransducerModuleId = p.TransducerModuleId AND t1_4.CategoryId = 1 AND t1_4.TestTypeId = 4 AND t1_4.DataFlag = 1 LIMIT 1) AS Result1_4,
                (SELECT t1_5.CategoryId FROM Tests t1_5 WHERE t1_5.TransducerModuleId = p.TransducerModuleId AND t1_5.CategoryId = 1 AND t1_5.TestTypeId = 5 AND t1_5.DataFlag = 1 LIMIT 1) AS CategoryId1_5,
                (SELECT t1_5.TestTypeId FROM Tests t1_5 WHERE t1_5.TransducerModuleId = p.TransducerModuleId AND t1_5.CategoryId = 1 AND t1_5.TestTypeId = 5 AND t1_5.DataFlag = 1 LIMIT 1) AS TestTypeId1_5,
                (SELECT t1_5.CreatedDate FROM Tests t1_5 WHERE t1_5.TransducerModuleId = p.TransducerModuleId AND t1_5.CategoryId = 1 AND t1_5.TestTypeId = 5 AND t1_5.DataFlag = 1 LIMIT 1) AS CreatedDate1_5,
                (SELECT t1_5.`Result` FROM Tests t1_5 WHERE t1_5.TransducerModuleId = p.TransducerModuleId AND t1_5.CategoryId = 1 AND t1_5.TestTypeId = 5 AND t1_5.DataFlag = 1 LIMIT 1) AS Result1_5,
	            (SELECT t1_6.CategoryId FROM Tests t1_6 WHERE t1_6.TransducerModuleId = p.TransducerModuleId AND t1_6.CategoryId = 1 AND t1_6.TestTypeId = 5 AND t1_6.DataFlag = 1 LIMIT 1) AS CategoryId1_5,
                (SELECT t1_6.TestTypeId FROM Tests t1_6 WHERE t1_6.TransducerModuleId = p.TransducerModuleId AND t1_6.CategoryId = 1 AND t1_6.TestTypeId = 5 AND t1_6.DataFlag = 1 LIMIT 1) AS TestTypeId1_5,
                (SELECT t1_6.CreatedDate FROM Tests t1_6 WHERE t1_6.TransducerModuleId = p.TransducerModuleId AND t1_6.CategoryId = 1 AND t1_6.TestTypeId = 5 AND t1_6.DataFlag = 1 LIMIT 1) AS CreatedDate1_5,
                (SELECT t1_6.`Result` FROM Tests t1_6 WHERE t1_6.TransducerModuleId = p.TransducerModuleId AND t1_6.CategoryId = 1 AND t1_6.TestTypeId = 5 AND t1_6.DataFlag = 1 LIMIT 1) AS Result1_6,
            -- 	
                (SELECT t2_1.CategoryId FROM Tests t2_1 WHERE t2_1.TransducerModuleId = p.TransducerModuleId AND t2_1.CategoryId = 1 AND t2_1.TestTypeId = 1 AND t2_1.DataFlag = 1 LIMIT 1) AS CategoryId2_1,
                (SELECT t2_1.TestTypeId FROM Tests t2_1 WHERE t2_1.TransducerModuleId = p.TransducerModuleId AND t2_1.CategoryId = 1 AND t2_1.TestTypeId = 1 AND t2_1.DataFlag = 1 LIMIT 1) AS TestTypeId2_1,
                (SELECT t2_1.CreatedDate FROM Tests t2_1 WHERE t2_1.TransducerModuleId = p.TransducerModuleId AND t2_1.CategoryId = 1 AND t2_1.TestTypeId = 1 AND t2_1.DataFlag = 1 LIMIT 1) AS CreatedDate2_1,
                (SELECT t2_1.`Result` FROM Tests t2_1 WHERE t2_1.TransducerModuleId = p.TransducerModuleId AND t2_1.CategoryId = 1 AND t2_1.TestTypeId = 1 AND t2_1.DataFlag = 1 LIMIT 1) AS Result2_1,
                (SELECT t2_2.CategoryId FROM Tests t2_2 WHERE t2_2.TransducerModuleId = p.TransducerModuleId AND t2_2.CategoryId = 1 AND t2_2.TestTypeId = 2 AND t2_2.DataFlag = 1 LIMIT 1) AS CategoryId2_2,
                (SELECT t2_2.TestTypeId FROM Tests t2_2 WHERE t2_2.TransducerModuleId = p.TransducerModuleId AND t2_2.CategoryId = 1 AND t2_2.TestTypeId = 2 AND t2_2.DataFlag = 1 LIMIT 1) AS TestTypeId2_2,
                (SELECT t2_2.CreatedDate FROM Tests t2_2 WHERE t2_2.TransducerModuleId = p.TransducerModuleId AND t2_2.CategoryId = 1 AND t2_2.TestTypeId = 2 AND t2_2.DataFlag = 1 LIMIT 1) AS CreatedDate2_2,
                (SELECT t2_2.`Result` FROM Tests t2_2 WHERE t2_2.TransducerModuleId = p.TransducerModuleId AND t2_2.CategoryId = 1 AND t2_2.TestTypeId = 2 AND t2_2.DataFlag = 1 LIMIT 1) AS Result2_2,
                (SELECT t2_3.CategoryId FROM Tests t2_3 WHERE t2_3.TransducerModuleId = p.TransducerModuleId AND t2_3.CategoryId = 1 AND t2_3.TestTypeId = 3 AND t2_3.DataFlag = 1 LIMIT 1) AS CategoryId2_3,
                (SELECT t2_3.TestTypeId FROM Tests t2_3 WHERE t2_3.TransducerModuleId = p.TransducerModuleId AND t2_3.CategoryId = 1 AND t2_3.TestTypeId = 3 AND t2_3.DataFlag = 1 LIMIT 1) AS TestTypeId2_3,
                (SELECT t2_3.CreatedDate FROM Tests t2_3 WHERE t2_3.TransducerModuleId = p.TransducerModuleId AND t2_3.CategoryId = 1 AND t2_3.TestTypeId = 3 AND t2_3.DataFlag = 1 LIMIT 1) AS CreatedDate2_3,
                (SELECT t2_3.`Result` FROM Tests t2_3 WHERE t2_3.TransducerModuleId = p.TransducerModuleId AND t2_3.CategoryId = 1 AND t2_3.TestTypeId = 3 AND t2_3.DataFlag = 1 LIMIT 1) AS Result2_3,
                (SELECT t2_4.CategoryId FROM Tests t2_4 WHERE t2_4.TransducerModuleId = p.TransducerModuleId AND t2_4.CategoryId = 1 AND t2_4.TestTypeId = 4 AND t2_4.DataFlag = 1 LIMIT 1) AS CategoryId2_4,
                (SELECT t2_4.TestTypeId FROM Tests t2_4 WHERE t2_4.TransducerModuleId = p.TransducerModuleId AND t2_4.CategoryId = 1 AND t2_4.TestTypeId = 4 AND t2_4.DataFlag = 1 LIMIT 1) AS TestTypeId2_4,
                (SELECT t2_4.CreatedDate FROM Tests t2_4 WHERE t2_4.TransducerModuleId = p.TransducerModuleId AND t2_4.CategoryId = 1 AND t2_4.TestTypeId = 4 AND t2_4.DataFlag = 1 LIMIT 1) AS CreatedDate2_4,
                (SELECT t2_4.`Result` FROM Tests t2_4 WHERE t2_4.TransducerModuleId = p.TransducerModuleId AND t2_4.CategoryId = 1 AND t2_4.TestTypeId = 4 AND t2_4.DataFlag = 1 LIMIT 1) AS Result2_4,
                (SELECT t2_5.CategoryId FROM Tests t2_5 WHERE t2_5.TransducerModuleId = p.TransducerModuleId AND t2_5.CategoryId = 1 AND t2_5.TestTypeId = 5 AND t2_5.DataFlag = 1 LIMIT 1) AS CategoryId2_5,
                (SELECT t2_5.TestTypeId FROM Tests t2_5 WHERE t2_5.TransducerModuleId = p.TransducerModuleId AND t2_5.CategoryId = 1 AND t2_5.TestTypeId = 5 AND t2_5.DataFlag = 1 LIMIT 1) AS TestTypeId2_5,
                (SELECT t2_5.CreatedDate FROM Tests t2_5 WHERE t2_5.TransducerModuleId = p.TransducerModuleId AND t2_5.CategoryId = 1 AND t2_5.TestTypeId = 5 AND t2_5.DataFlag = 1 LIMIT 1) AS CreatedDate2_5,
                (SELECT t2_5.`Result` FROM Tests t2_5 WHERE t2_5.TransducerModuleId = p.TransducerModuleId AND t2_5.CategoryId = 1 AND t2_5.TestTypeId = 5 AND t2_5.DataFlag = 1 LIMIT 1) AS Result2_5,
	            (SELECT t2_6.CategoryId FROM Tests t2_6 WHERE t2_6.TransducerModuleId = p.TransducerModuleId AND t2_6.CategoryId = 1 AND t2_6.TestTypeId = 5 AND t2_6.DataFlag = 1 LIMIT 1) AS CategoryId2_5,
                (SELECT t2_6.TestTypeId FROM Tests t2_6 WHERE t2_6.TransducerModuleId = p.TransducerModuleId AND t2_6.CategoryId = 1 AND t2_6.TestTypeId = 5 AND t2_6.DataFlag = 1 LIMIT 1) AS TestTypeId2_5,
                (SELECT t2_6.CreatedDate FROM Tests t2_6 WHERE t2_6.TransducerModuleId = p.TransducerModuleId AND t2_6.CategoryId = 1 AND t2_6.TestTypeId = 5 AND t2_6.DataFlag = 1 LIMIT 1) AS CreatedDate2_5,
                (SELECT t2_6.`Result` FROM Tests t2_6 WHERE t2_6.TransducerModuleId = p.TransducerModuleId AND t2_6.CategoryId = 1 AND t2_6.TestTypeId = 5 AND t2_6.DataFlag = 1 LIMIT 1) AS Result2_6,
            -- 	
                (SELECT t3_1.CategoryId FROM Tests t3_1 WHERE t3_1.TransducerModuleId = p.TransducerModuleId AND t3_1.CategoryId = 1 AND t3_1.TestTypeId = 1 AND t3_1.DataFlag = 1 LIMIT 1) AS CategoryId3_1,
                (SELECT t3_1.TestTypeId FROM Tests t3_1 WHERE t3_1.TransducerModuleId = p.TransducerModuleId AND t3_1.CategoryId = 1 AND t3_1.TestTypeId = 1 AND t3_1.DataFlag = 1 LIMIT 1) AS TestTypeId3_1,
                (SELECT t3_1.CreatedDate FROM Tests t3_1 WHERE t3_1.TransducerModuleId = p.TransducerModuleId AND t3_1.CategoryId = 1 AND t3_1.TestTypeId = 1 AND t3_1.DataFlag = 1 LIMIT 1) AS CreatedDate3_1,
                (SELECT t3_1.`Result` FROM Tests t3_1 WHERE t3_1.TransducerModuleId = p.TransducerModuleId AND t3_1.CategoryId = 1 AND t3_1.TestTypeId = 1 AND t3_1.DataFlag = 1 LIMIT 1) AS Result3_1,
                (SELECT t3_2.CategoryId FROM Tests t3_2 WHERE t3_2.TransducerModuleId = p.TransducerModuleId AND t3_2.CategoryId = 1 AND t3_2.TestTypeId = 2 AND t3_2.DataFlag = 1 LIMIT 1) AS CategoryId3_2,
                (SELECT t3_2.TestTypeId FROM Tests t3_2 WHERE t3_2.TransducerModuleId = p.TransducerModuleId AND t3_2.CategoryId = 1 AND t3_2.TestTypeId = 2 AND t3_2.DataFlag = 1 LIMIT 1) AS TestTypeId3_2,
                (SELECT t3_2.CreatedDate FROM Tests t3_2 WHERE t3_2.TransducerModuleId = p.TransducerModuleId AND t3_2.CategoryId = 1 AND t3_2.TestTypeId = 2 AND t3_2.DataFlag = 1 LIMIT 1) AS CreatedDate3_2,
                (SELECT t3_2.`Result` FROM Tests t3_2 WHERE t3_2.TransducerModuleId = p.TransducerModuleId AND t3_2.CategoryId = 1 AND t3_2.TestTypeId = 2 AND t3_2.DataFlag = 1 LIMIT 1) AS Result3_2,
                (SELECT t3_3.CategoryId FROM Tests t3_3 WHERE t3_3.TransducerModuleId = p.TransducerModuleId AND t3_3.CategoryId = 1 AND t3_3.TestTypeId = 3 AND t3_3.DataFlag = 1 LIMIT 1) AS CategoryId3_3,
                (SELECT t3_3.TestTypeId FROM Tests t3_3 WHERE t3_3.TransducerModuleId = p.TransducerModuleId AND t3_3.CategoryId = 1 AND t3_3.TestTypeId = 3 AND t3_3.DataFlag = 1 LIMIT 1) AS TestTypeId3_3,
                (SELECT t3_3.CreatedDate FROM Tests t3_3 WHERE t3_3.TransducerModuleId = p.TransducerModuleId AND t3_3.CategoryId = 1 AND t3_3.TestTypeId = 3 AND t3_3.DataFlag = 1 LIMIT 1) AS CreatedDate3_3,
                (SELECT t3_3.`Result` FROM Tests t3_3 WHERE t3_3.TransducerModuleId = p.TransducerModuleId AND t3_3.CategoryId = 1 AND t3_3.TestTypeId = 3 AND t3_3.DataFlag = 1 LIMIT 1) AS Result3_3,
                (SELECT t3_4.CategoryId FROM Tests t3_4 WHERE t3_4.TransducerModuleId = p.TransducerModuleId AND t3_4.CategoryId = 1 AND t3_4.TestTypeId = 4 AND t3_4.DataFlag = 1 LIMIT 1) AS CategoryId3_4,
                (SELECT t3_4.TestTypeId FROM Tests t3_4 WHERE t3_4.TransducerModuleId = p.TransducerModuleId AND t3_4.CategoryId = 1 AND t3_4.TestTypeId = 4 AND t3_4.DataFlag = 1 LIMIT 1) AS TestTypeId3_4,
                (SELECT t3_4.CreatedDate FROM Tests t3_4 WHERE t3_4.TransducerModuleId = p.TransducerModuleId AND t3_4.CategoryId = 1 AND t3_4.TestTypeId = 4 AND t3_4.DataFlag = 1 LIMIT 1) AS CreatedDate3_4,
                (SELECT t3_4.`Result` FROM Tests t3_4 WHERE t3_4.TransducerModuleId = p.TransducerModuleId AND t3_4.CategoryId = 1 AND t3_4.TestTypeId = 4 AND t3_4.DataFlag = 1 LIMIT 1) AS Result3_4,
                (SELECT t3_5.CategoryId FROM Tests t3_5 WHERE t3_5.TransducerModuleId = p.TransducerModuleId AND t3_5.CategoryId = 1 AND t3_5.TestTypeId = 5 AND t3_5.DataFlag = 1 LIMIT 1) AS CategoryId3_5,
                (SELECT t3_5.TestTypeId FROM Tests t3_5 WHERE t3_5.TransducerModuleId = p.TransducerModuleId AND t3_5.CategoryId = 1 AND t3_5.TestTypeId = 5 AND t3_5.DataFlag = 1 LIMIT 1) AS TestTypeId3_5,
                (SELECT t3_5.CreatedDate FROM Tests t3_5 WHERE t3_5.TransducerModuleId = p.TransducerModuleId AND t3_5.CategoryId = 1 AND t3_5.TestTypeId = 5 AND t3_5.DataFlag = 1 LIMIT 1) AS CreatedDate3_5,
                (SELECT t3_5.`Result` FROM Tests t3_5 WHERE t3_5.TransducerModuleId = p.TransducerModuleId AND t3_5.CategoryId = 1 AND t3_5.TestTypeId = 5 AND t3_5.DataFlag = 1 LIMIT 1) AS Result3_5,
	            (SELECT t3_6.CategoryId FROM Tests t3_6 WHERE t3_6.TransducerModuleId = p.TransducerModuleId AND t3_6.CategoryId = 1 AND t3_6.TestTypeId = 5 AND t3_6.DataFlag = 1 LIMIT 1) AS CategoryId3_6,
                (SELECT t3_6.TestTypeId FROM Tests t3_6 WHERE t3_6.TransducerModuleId = p.TransducerModuleId AND t3_6.CategoryId = 1 AND t3_6.TestTypeId = 5 AND t3_6.DataFlag = 1 LIMIT 1) AS TestTypeId3_6,
                (SELECT t3_6.CreatedDate FROM Tests t3_6 WHERE t3_6.TransducerModuleId = p.TransducerModuleId AND t3_6.CategoryId = 1 AND t3_6.TestTypeId = 5 AND t3_6.DataFlag = 1 LIMIT 1) AS CreatedDate3_6,
                (SELECT t3_6.`Result` FROM Tests t3_6 WHERE t3_6.TransducerModuleId = p.TransducerModuleId AND t3_6.CategoryId = 1 AND t3_6.TestTypeId = 5 AND t3_6.DataFlag = 1 LIMIT 1) AS Result3_6
            FROM Probes p
            LEFT Join TransducerModules tdm on p.TransducerModuleId = tdm.Id
            LEFT Join MotorModules mtm on p.MotorModuleId = mtm.Id;
            ";
            #endregion

            #region SELECT_SQL_JOIN_3
            string sqlJoin = @"
            SELECT
                p.Id, 
                p.ProbeSn,
                tdm.TransducerModuleSn,
                mtm.MotorModuleSn,
                --Test 1
                t1.CategoryId AS CategoryId1_1,
                t1.TestTypeId AS TestTypeId1_1,
                t1.CreatedDate AS CreatedDate1_1,
                t1.`Result` AS Result1_1,
                --Test 2
                t2.CategoryId AS CategoryId1_2,
                t2.TestTypeId AS TestTypeId1_2,
                t2.CreatedDate AS CreatedDate1_2,
                t2.`Result` AS Result1_2,
                --Test 3
                t3.CategoryId AS CategoryId1_3,
                t3.TestTypeId AS TestTypeId1_3,
                t3.CreatedDate AS CreatedDate1_3,
                t3.`Result` AS Result1_3,
                --Test 4
                t4.CategoryId AS CategoryId1_4,
                t4.TestTypeId AS TestTypeId1_4,
                t4.CreatedDate AS CreatedDate1_4,
                t4.`Result` AS Result1_4,
                --Test 5
                t5.CategoryId AS CategoryId1_5,
                t5.TestTypeId AS TestTypeId1_5,
                t5.CreatedDate AS CreatedDate1_5,
                t5.`Result` AS Result1_5,
                --Test 6
                t6.CategoryId AS CategoryId1_6,
                t6.TestTypeId AS TestTypeId1_6,
                t6.CreatedDate AS CreatedDate1_6,
                t6.`Result` AS Result1_6,
                --Test 7
                t7.CategoryId AS CategoryId2_1,
                t7.TestTypeId AS TestTypeId2_1,
                t7.CreatedDate AS CreatedDate2_1,
                t7.`Result` AS Result2_1,
                --Test 8
                t8.CategoryId AS CategoryId2_2,
                t8.TestTypeId AS TestTypeId2_2,
                t8.CreatedDate AS CreatedDate2_2,
                t8.`Result` AS Result2_2,
                --Test 9
                t9.CategoryId AS CategoryId2_3,
                t9.TestTypeId AS TestTypeId2_3,
                t9.CreatedDate AS CreatedDate2_3,
                t9.`Result` AS Result2_3,
                --Test 10
                t10.CategoryId AS CategoryId2_4,
                t10.TestTypeId AS TestTypeId2_4,
                t10.CreatedDate AS CreatedDate2_4,
                t10.`Result` AS Result2_4,
                --Test 11
                t11.CategoryId AS CategoryId2_5,
                t11.TestTypeId AS TestTypeId2_5,
                t11.CreatedDate AS CreatedDate2_5,
                t11.`Result` AS Result2_5,
                --Test 12
                t12.CategoryId AS CategoryId2_6,
                t12.TestTypeId AS TestTypeId2_6,
                t12.CreatedDate AS CreatedDate2_6,
                t12.`Result` AS Result2_6,
                --Test 13
                t13.CategoryId AS CategoryId3_1,
                t13.TestTypeId AS TestTypeId3_1,
                t13.CreatedDate AS CreatedDate3_1,
                t13.`Result` AS Result3_1,
                --Test 14
                t14.CategoryId AS CategoryId3_2,
                t14.TestTypeId AS TestTypeId3_2,
                t14.CreatedDate AS CreatedDate3_2,
                t14.`Result` AS Result3_2,
                --Test 15
                t15.CategoryId AS CategoryId3_3,
                t15.TestTypeId AS TestTypeId3_3,
                t15.CreatedDate AS CreatedDate3_3,
                t15.`Result` AS Result3_3,
                --Test 16
                t16.CategoryId AS CategoryId3_4,
                t16.TestTypeId AS TestTypeId3_4,
                t16.CreatedDate AS CreatedDate3_4,
                t16.`Result` AS Result3_4,
                --Test 17
                t17.CategoryId AS CategoryId3_5,
                t17.TestTypeId AS TestTypeId3_5,
                t17.CreatedDate AS CreatedDate3_5,
                t17.`Result` AS Result3_5,
                --Test 18
                t18.CategoryId AS CategoryId3_6,
                t18.TestTypeId AS TestTypeId3_6,
                t18.CreatedDate AS CreatedDate3_6,
                t18.`Result` AS Result3_6
            FROM Probes p
                        LEFT JOIN TransducerModules tdm ON p.TransducerModuleId = tdm.Id
            LEFT JOIN MotorModules mtm ON p.MotorModuleId = mtm.Id
                        -- Test 1
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 1 AND DataFlag = 1) AS t1 ON p.TransducerModuleId = t1.TransducerModuleId AND t1.TestTypeId = 1
                        -- Test 2
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 1 AND DataFlag = 1) AS t2 ON p.TransducerModuleId = t2.TransducerModuleId AND t2.TestTypeId = 2
                        -- Test 3
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 1 AND DataFlag = 1) AS t3 ON p.TransducerModuleId = t3.TransducerModuleId AND t3.TestTypeId = 3
                        -- Test 4
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 1 AND DataFlag = 1) AS t4 ON p.TransducerModuleId = t4.TransducerModuleId AND t4.TestTypeId = 4
                        -- Test 5
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 1 AND DataFlag = 1) AS t5 ON p.TransducerModuleId = t5.TransducerModuleId AND t5.TestTypeId = 5
                        -- Test 6
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 1 AND DataFlag = 1) AS t6 ON p.TransducerModuleId = t6.TransducerModuleId AND t6.TestTypeId = 6
                        -- Test 7
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 2 AND DataFlag = 1) AS t7 ON p.TransducerModuleId = t7.TransducerModuleId AND t7.TestTypeId = 1
                        -- Test 8
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 2 AND DataFlag = 1) AS t8 ON p.TransducerModuleId = t8.TransducerModuleId AND t8.TestTypeId = 2
                        -- Test 9
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 2 AND DataFlag = 1) AS t9 ON p.TransducerModuleId = t9.TransducerModuleId AND t9.TestTypeId = 3
                        -- Test 10
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 2 AND DataFlag = 1) AS t10 ON p.TransducerModuleId = t10.TransducerModuleId AND t10.TestTypeId = 4
                        -- Test 11
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 2 AND DataFlag = 1) AS t11 ON p.TransducerModuleId = t11.TransducerModuleId AND t11.TestTypeId = 5
                        -- Test 12
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 2 AND DataFlag = 1) AS t12 ON p.TransducerModuleId = t12.TransducerModuleId AND t12.TestTypeId = 6
                        -- Test 13
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 3 AND DataFlag = 1) AS t13 ON p.TransducerModuleId = t13.TransducerModuleId AND t13.TestTypeId = 1
                        -- Test 14
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 3 AND DataFlag = 1) AS t14 ON p.TransducerModuleId = t14.TransducerModuleId AND t14.TestTypeId = 2
                        -- Test 15
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 3 AND DataFlag = 1) AS t15 ON p.TransducerModuleId = t15.TransducerModuleId AND t15.TestTypeId = 3
                        -- Test 16
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 3 AND DataFlag = 1) AS t16 ON p.TransducerModuleId = t16.TransducerModuleId AND t16.TestTypeId = 4
                        -- Test 17
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 3 AND DataFlag = 1) AS t17 ON p.TransducerModuleId = t17.TransducerModuleId AND t17.TestTypeId = 5
                        -- Test 18
            LEFT JOIN(SELECT TransducerModuleId, CategoryId, TestTypeId, CreatedDate, `Result` FROM Tests WHERE CategoryId = 3 AND DataFlag = 1) AS t18 ON p.TransducerModuleId = t18.TransducerModuleId AND t18.TestTypeId = 6;
            ";
            #endregion


#endif

#if SELECT_SQL_4
            #region SELECT_SQL_4
            string sql = @"
            SELECT 
                p.Id, 
                p.ProbeSn,
                tdm.TransducerModuleSn,
                mtm.MotorModuleSn,
                (SELECT t1_1.CategoryId FROM Tests t1_1 WHERE t1_1.TransducerModuleId = p.TransducerModuleId AND t1_1.CategoryId = 1 AND t1_1.TestTypeId = 1 AND t1_1.DataFlag = 1 LIMIT 1) AS CategoryId1_1,
                (SELECT t1_1.TestTypeId FROM Tests t1_1 WHERE t1_1.TransducerModuleId = p.TransducerModuleId AND t1_1.CategoryId = 1 AND t1_1.TestTypeId = 1 AND t1_1.DataFlag = 1 LIMIT 1) AS TestTypeId1_1,
                (SELECT t1_1.CreatedDate FROM Tests t1_1 WHERE t1_1.TransducerModuleId = p.TransducerModuleId AND t1_1.CategoryId = 1 AND t1_1.TestTypeId = 1 AND t1_1.DataFlag = 1 LIMIT 1) AS CreatedDate1_1,
                (SELECT t1_1.`Result` FROM Tests t1_1 WHERE t1_1.TransducerModuleId = p.TransducerModuleId AND t1_1.CategoryId = 1 AND t1_1.TestTypeId = 1 AND t1_1.DataFlag = 1 LIMIT 1) AS Result1_1,
                (SELECT t1_2.CategoryId FROM Tests t1_2 WHERE t1_2.TransducerModuleId = p.TransducerModuleId AND t1_2.CategoryId = 1 AND t1_2.TestTypeId = 2 AND t1_2.DataFlag = 1 LIMIT 1) AS CategoryId1_2,
                (SELECT t1_2.TestTypeId FROM Tests t1_2 WHERE t1_2.TransducerModuleId = p.TransducerModuleId AND t1_2.CategoryId = 1 AND t1_2.TestTypeId = 2 AND t1_2.DataFlag = 1 LIMIT 1) AS TestTypeId1_2,
                (SELECT t1_2.CreatedDate FROM Tests t1_2 WHERE t1_2.TransducerModuleId = p.TransducerModuleId AND t1_2.CategoryId = 1 AND t1_2.TestTypeId = 2 AND t1_2.DataFlag = 1 LIMIT 1) AS CreatedDate1_2,
                (SELECT t1_2.`Result` FROM Tests t1_2 WHERE t1_2.TransducerModuleId = p.TransducerModuleId AND t1_2.CategoryId = 1 AND t1_2.TestTypeId = 2 AND t1_2.DataFlag = 1 LIMIT 1) AS Result1_2,
                (SELECT t1_3.CategoryId FROM Tests t1_3 WHERE t1_3.TransducerModuleId = p.TransducerModuleId AND t1_3.CategoryId = 1 AND t1_3.TestTypeId = 3 AND t1_3.DataFlag = 1 LIMIT 1) AS CategoryId1_3,
                (SELECT t1_3.TestTypeId FROM Tests t1_3 WHERE t1_3.TransducerModuleId = p.TransducerModuleId AND t1_3.CategoryId = 1 AND t1_3.TestTypeId = 3 AND t1_3.DataFlag = 1 LIMIT 1) AS TestTypeId1_3,
                (SELECT t1_3.CreatedDate FROM Tests t1_3 WHERE t1_3.TransducerModuleId = p.TransducerModuleId AND t1_3.CategoryId = 1 AND t1_3.TestTypeId = 3 AND t1_3.DataFlag = 1 LIMIT 1) AS CreatedDate1_3,
                (SELECT t1_3.`Result` FROM Tests t1_3 WHERE t1_3.TransducerModuleId = p.TransducerModuleId AND t1_3.CategoryId = 1 AND t1_3.TestTypeId = 3 AND t1_3.DataFlag = 1 LIMIT 1) AS Result1_3,
                (SELECT t2_1.CategoryId FROM Tests t2_1 WHERE t2_1.TransducerModuleId = p.TransducerModuleId AND t2_1.CategoryId = 1 AND t2_1.TestTypeId = 1 AND t2_1.DataFlag = 1 LIMIT 1) AS CategoryId2_1,
                (SELECT t2_1.TestTypeId FROM Tests t2_1 WHERE t2_1.TransducerModuleId = p.TransducerModuleId AND t2_1.CategoryId = 1 AND t2_1.TestTypeId = 1 AND t2_1.DataFlag = 1 LIMIT 1) AS TestTypeId2_1,
                (SELECT t2_1.CreatedDate FROM Tests t2_1 WHERE t2_1.TransducerModuleId = p.TransducerModuleId AND t2_1.CategoryId = 1 AND t2_1.TestTypeId = 1 AND t2_1.DataFlag = 1 LIMIT 1) AS CreatedDate2_1,
                (SELECT t2_1.`Result` FROM Tests t2_1 WHERE t2_1.TransducerModuleId = p.TransducerModuleId AND t2_1.CategoryId = 1 AND t2_1.TestTypeId = 1 AND t2_1.DataFlag = 1 LIMIT 1) AS Result2_1,
                (SELECT t2_2.CategoryId FROM Tests t2_2 WHERE t2_2.TransducerModuleId = p.TransducerModuleId AND t2_2.CategoryId = 1 AND t2_2.TestTypeId = 2 AND t2_2.DataFlag = 1 LIMIT 1) AS CategoryId2_2,
                (SELECT t2_2.TestTypeId FROM Tests t2_2 WHERE t2_2.TransducerModuleId = p.TransducerModuleId AND t2_2.CategoryId = 1 AND t2_2.TestTypeId = 2 AND t2_2.DataFlag = 1 LIMIT 1) AS TestTypeId2_2,
                (SELECT t2_2.CreatedDate FROM Tests t2_2 WHERE t2_2.TransducerModuleId = p.TransducerModuleId AND t2_2.CategoryId = 1 AND t2_2.TestTypeId = 2 AND t2_2.DataFlag = 1 LIMIT 1) AS CreatedDate2_2,
                (SELECT t2_2.`Result` FROM Tests t2_2 WHERE t2_2.TransducerModuleId = p.TransducerModuleId AND t2_2.CategoryId = 1 AND t2_2.TestTypeId = 2 AND t2_2.DataFlag = 1 LIMIT 1) AS Result2_2,
                (SELECT t2_3.CategoryId FROM Tests t2_3 WHERE t2_3.TransducerModuleId = p.TransducerModuleId AND t2_3.CategoryId = 1 AND t2_3.TestTypeId = 3 AND t2_3.DataFlag = 1 LIMIT 1) AS CategoryId2_3,
                (SELECT t2_3.TestTypeId FROM Tests t2_3 WHERE t2_3.TransducerModuleId = p.TransducerModuleId AND t2_3.CategoryId = 1 AND t2_3.TestTypeId = 3 AND t2_3.DataFlag = 1 LIMIT 1) AS TestTypeId2_3,
                (SELECT t2_3.CreatedDate FROM Tests t2_3 WHERE t2_3.TransducerModuleId = p.TransducerModuleId AND t2_3.CategoryId = 1 AND t2_3.TestTypeId = 3 AND t2_3.DataFlag = 1 LIMIT 1) AS CreatedDate2_3,
                (SELECT t2_3.`Result` FROM Tests t2_3 WHERE t2_3.TransducerModuleId = p.TransducerModuleId AND t2_3.CategoryId = 1 AND t2_3.TestTypeId = 3 AND t2_3.DataFlag = 1 LIMIT 1) AS Result2_3
            FROM Probes p
            LEFT Join TransducerModules tdm on p.TransducerModuleId = tdm.Id
            LEFT Join MotorModules mtm on p.MotorModuleId = mtm.Id;
            ";
            #endregion

            #region SELECT_SQL_JOIN_4
            string sqlJoin = @"
            SELECT 
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
                Tests t6 ON p.TransducerModuleId = t6.TransducerModuleId AND t6.CategoryId = 2 AND t6.TestTypeId = 3 AND t6.DataFlag = 1;
            ";
            #endregion

            ////var result = context.Database.SqlQuery(sqlJoin).ToList();
            var ProbeResults = context.Probes.FromSqlRaw(sqlJoin);
            ////var ProbeResults = context.Database.SqlQuery(sqlJoin).ToList();

            //foreach (var probe in ProbeResults)
            //{
            //    Console.WriteLine(probe.ToString());
            //    //Console.WriteLine($"{probe.ProbeSn}.{probe..Result1_1}.{probe.Result1_2}.{probe.Result1_3}.{probe.Result2_1}.{probe.Result2_2}.{probe.Result2_3}");
            //}
            Console.WriteLine("ProbeResult.Count : " + ProbeResults.Count());

#endif

            //var probeViews = context.Probes.Select(probe => new ProbeView
            //{
            //    ProbeSN = probe.ProbeSn,
            //    TransducerModuleSN = probe.TransducerModuleSN,
            //    TransducerSN = probe.TransducerSN,
            //    MotorModuleSn = probe.MotorModuleSn,
            //    Category1Results = context.TestResults
            //        .Where(result => result.ProbeId == probe.Id && result.CategoryId == 1)
            //        .ToList(),
            //    Category2Results = context.TestResults
            //        .Where(result => result.ProbeId == probe.Id && result.CategoryId == 2)
            //        .ToList()
            //}).ToList();

            stopwatch.Stop();
            Console.WriteLine($"Execution time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }

    private static string MKRandom(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        // 랜덤 숫자 생성기 인스턴스 생성
        Random random = new Random();

        // 랜덤한 10자리 문자열 생성
        char[] stringChars = new char[length];
        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
        string randomString = new string(stringChars);

        //Console.WriteLine("랜덤한 10자리 문자열: " + randomString);

        return randomString;
    }

    private static string MKSHA256()
    {
        string input = MKRandom(20);
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            // 해시 값을 문자열로 변환하여 출력
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2")); // 각 바이트를 16진수로 변환하여 추가
            }
            string hashString = sb.ToString();
            //Console.WriteLine("SHA-256 해시 값: " + hashString);
            return hashString;
        }
    }
}