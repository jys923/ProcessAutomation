//#define INSERT_MASTER
//#define INSERT_DATA
//#define SELECT_LINQ
//#define SELECT_SQL
//#define SELECT_SQL_DYNAMIC
#define SELECT_SQL_DYNAMIC_2
//#define PRINT

using EFCoreSample.Data;
using EFCoreSample.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Dynamic;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello EFCoreSample!");

        int maxCnt = 100000;
        int numberOfResults = 5;

        Random random = new Random();
        using (var db = new EFCoreSampleDbContext())
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
            db.InspectTypes.Add(new InspectType { Name = "1st test", Detail = "align" });
            db.InspectTypes.Add(new InspectType { Name = "2nd test", Detail = "center" });
            db.InspectTypes.Add(new InspectType { Name = "3rd test", Detail = "circle" });
            db.InspectTypes.Add(new InspectType { Name = "4th test", Detail = "red" });
            db.InspectTypes.Add(new InspectType { Name = "5th test", Detail = "green" });
            db.InspectorTypes.Add(new InspectorType { Name = "yoon", PcNo = 1 });
            db.InspectorTypes.Add(new InspectorType { Name = "sang", PcNo = 2 });
            db.InspectorTypes.Add(new InspectorType { Name = "ko", PcNo = 3 });
            db.InspectorTypes.Add(new InspectorType { Name = "kwon", PcNo = 4 });
            db.SaveChanges();
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
            
            #region inspect
            for (int i = 0; i < maxCnt; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var inspect = new Inspect
                    {
                        InspectTypeId = j + 1,
                        InspectorTypeId = random.Next(1, 5),
                        OriginalImg = "/img/" + DateTime.Now.ToString("yyyyMMdd") + "/OImg.png",
                        ChangedImg = "/img/" + DateTime.Now.ToString("yyyyMMdd") + "/CImg.png",
                        ChangedImgMetadata = "red circle",
                        Result = random.Next(0, 2),
                        //ProbeSNId = i + 1,
                    };
                    inspect.ProbeSNId = i + 1;
                    db.Inspects.Add(inspect);
                }
            }
            db.SaveChanges();
            #endregion
#endif

#if false
            #region 기본 LINQ
            //List<Inspect> list = db.Inspects.ToList();
            DbSet<Inspect> inspects = db.Inspects;
            List<Inspect> list = inspects.ToList();
            IEnumerable<Inspect> enums = inspects.AsEnumerable();
            #endregion
#endif

#if false
            #region 외래키 null 나올떄 대처
            List<Inspect> list = db.Inspects
            .Include(u => u.InspectType)
            .Include(u => u.InspectorType)
            .Include(u => u.ProbeSN)
            .Include(u => u.ProbeSN.ProbeType)
            .Include(u => u.ProbeSN.ProbeSNType)
            .ToList();
            foreach (var inspect in list)
            {
                //Console.WriteLine($"{inspect.ProbeSN}.{inspect.Result1}.{inspect.Result2}.{inspect.Result3}.{inspect.Result4}.{inspect.Result5}");
                Console.WriteLine(inspect.ToString());
            }
            #endregion
#endif

#if false
            #region sort get Last
            var query3 = (from main in db.Inspects
                          group main by main.ProbeSNId into g
                          select new
                          {
                              ProbeSNId = g.Key,
                              Result1 = (from sub1 in g
                                         where sub1.InspectTypeId == 1
                                         orderby sub1.Id descending
                                         select sub1.Result).FirstOrDefault(),
                              Result2 = (from sub2 in g
                                         where sub2.InspectTypeId == 2
                                         orderby sub2.Id descending
                                         select sub2.Result).FirstOrDefault(),
                              Result3 = (from sub3 in g
                                         where sub3.InspectTypeId == 3
                                         orderby sub3.Id descending
                                         select sub3.Result).FirstOrDefault(),
                              Result4 = (from sub4 in g
                                         where sub4.InspectTypeId == 4
                                         orderby sub4.Id descending
                                         select sub4.Result).FirstOrDefault(),
                              Result5 = (from sub5 in g
                                         where sub5.InspectTypeId == 5
                                         orderby sub5.Id descending
                                         select sub5.Result).FirstOrDefault()
                          }).ToList();
            #endregion
#endif

#if false
            #region linQ join err why?
            var query2 = from main in db.Inspects
                 join ps in db.ProbeSNs on main.ProbeSNId equals ps.Id
                 join pst in db.ProbeSNTypes on ps.ProbeSNTypeId equals pst.Id
                 join pt in db.ProbeTypes on ps.ProbeTypeId equals pt.Id
                 group main by main.ProbeSNId into g
                 select new
                 {
                     ProbeSN = string.Concat(pt.Code, pst.DateTime,
                                              g.Key.ToString().PadLeft(3, '0'),
                                              ps.ProbeSeqNo.ToString().PadLeft(5, '0')),
                     Result1 = (from i in g where i.InspectTypeId == 1 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                     Result2 = (from i in g where i.InspectTypeId == 2 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                     Result3 = (from i in g where i.InspectTypeId == 3 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                     Result4 = (from i in g where i.InspectTypeId == 4 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                     Result5 = (from i in g where i.InspectTypeId == 5 && i.DataFlag == 1 select i.Result).FirstOrDefault()
                 }; 
    #endregion
#endif

#if SELECT_LINQ
            #region select linq join 30s
            var query = from main in db.Inspects
                        join ps in db.ProbeSNs on main.ProbeSNId equals ps.Id
                        join pst in db.ProbeSNTypes on ps.ProbeSNTypeId equals pst.Id
                        join pt in db.ProbeTypes on ps.ProbeTypeId equals pt.Id
                        group main by main.ProbeSNId into g
                        select new
                        {
                            ProbeSN = string.Concat(g.First().ProbeSN.ProbeType.Code, g.First().ProbeSN.ProbeSNType.DateTime,
                                                     g.First().ProbeSN.ProbeSNType.PcNo.ToString().PadLeft(3, '0'),
                                                     g.First().ProbeSN.ProbeSeqNo.ToString().PadLeft(5, '0')),
                            Result1 = (from i in g where i.InspectTypeId == 1 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                            Result2 = (from i in g where i.InspectTypeId == 2 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                            Result3 = (from i in g where i.InspectTypeId == 3 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                            Result4 = (from i in g where i.InspectTypeId == 4 && i.DataFlag == 1 select i.Result).FirstOrDefault(),
                            Result5 = (from i in g where i.InspectTypeId == 5 && i.DataFlag == 1 select i.Result).FirstOrDefault()
                        };

            foreach (var inspect in query)
            {
                Console.WriteLine($"{inspect.ProbeSN}.{inspect.Result1}.{inspect.Result2}.{inspect.Result3}.{inspect.Result4}.{inspect.Result5}");
            }
            Console.WriteLine("query.Count : " + query.Count());
            #endregion
#endif

#if SELECT_SQL
            #region select SQL 8s
            //var url = "http://example.com";
            //var blogs = db.Inspects.FromSqlRaw($"SELECT * FROM Blogs WHERE Url = {url}");

            string sql = @"
                        SELECT CONCAT(pt.Code, pst.DateTime, LPAD(pst.PcNo, 3, '0'), LPAD(ps.ProbeSeqNo,5,'0')) AS ProbeSN,
                               (SELECT Result FROM Inspects WHERE ProbeSNId = main.ProbeSNId AND InspectTypeId = 1 AND DataFlag = 1) AS Result1,
                               (SELECT Result FROM Inspects WHERE ProbeSNId = main.ProbeSNId AND InspectTypeId = 2 AND DataFlag = 1) AS Result2,
                               (SELECT Result FROM Inspects WHERE ProbeSNId = main.ProbeSNId AND InspectTypeId = 3 AND DataFlag = 1) AS Result3,
                               (SELECT Result FROM Inspects WHERE ProbeSNId = main.ProbeSNId AND InspectTypeId = 4 AND DataFlag = 1) AS Result4,
                               (SELECT Result FROM Inspects WHERE ProbeSNId = main.ProbeSNId AND InspectTypeId = 5 AND DataFlag = 1) AS Result5
                        FROM (SELECT DISTINCT ProbeSNId FROM Inspects) AS main
                        JOIN ProbeSNs ps ON main.ProbeSNId = ps.Id
                        JOIN ProbeSNTypes pst ON ps.ProbeSNTypeId = pst.Id
                        JOIN ProbeTypes pt ON ps.ProbeTypeId = pt.Id";
            IQueryable<ProbeView> ProbeResults = db.ProbeViews.FromSqlRaw(sql);

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
            for (int i = 1; i <= numberOfResults; i++)
            {
                sqlBuilder.AppendLine($"(SELECT Result FROM Inspects WHERE ProbeSNId = main.ProbeSNId AND InspectTypeId = {i} AND DataFlag = 1) AS Result{i},");
            }
            // 마지막 줄에는 쉼표를 제거
            sqlBuilder.Remove(sqlBuilder.Length - 3, 1);

            string sql = $@"
            SELECT CONCAT(pt.Code, pst.DateTime, LPAD(pst.PcNo, 3, '0'), LPAD(ps.ProbeSeqNo,5,'0')) AS ProbeSN,
                   {sqlBuilder}
                FROM (SELECT DISTINCT ProbeSNId FROM Inspects) AS main
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
                        for (int i = 1; i <= numberOfResults; i++)
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
#endif

            stopwatch.Stop();
            Console.WriteLine($"Execution time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}