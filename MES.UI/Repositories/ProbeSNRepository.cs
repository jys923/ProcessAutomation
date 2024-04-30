using MES.UI.Data;
using MES.UI.Entities;
using MES.UI.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;

namespace MES.UI.Repositories
{
    internal class ProbeSNRepository : RepositoryBase<TransducerModule> , IProbeSNRepository
    {
        public ProbeSNRepository(MESDbContext context) : base(context)
        {
        }

        public IEnumerable<TransducerModuleView> GetProbeSN(int resultCnt)
        {
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

            List<TransducerModuleView> probeSNViews = new List<TransducerModuleView>();

            // 데이터베이스 연결 및 쿼리 실행
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        TransducerModuleView probeSNView = new TransducerModuleView
                        {
                            TransducerModuleSn = result.GetString(0), // 첫 번째 컬럼은 ProbeSN
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
            return probeSNViews;
            #endregion
        }
    }
}
