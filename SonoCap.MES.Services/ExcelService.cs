using MiniExcelLibs;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Converts;
using SonoCap.MES.Services.Interfaces;

namespace SonoCap.MES.Services
{
    public class ExcelService : IExcelService
    {
        public void ExportToExcel(IEnumerable<PTRView> data, string filePath)
        {
            if (data.Count() < 1)
                throw new ArgumentNullException(nameof(data));

            IEnumerable<IDictionary<string, object>> probes = PTRViewToExportProbe.ToDictionaryList(data);
            MiniExcel.SaveAs(filePath, probes);
        }

        public bool IsExcelFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                return false;
            }

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var excel = MiniExcel.Query(stream);
                    return excel != null;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Dictionary<string, List<SnDate>> ReadColumnsDataByHeaders(string filePath, List<string> headersToFind)
        {
            var result = new Dictionary<string, List<SnDate>>();

            if (!IsExcelFile(filePath))
            {
                Log.Information("유효한 Excel 파일이 아닙니다.");
                return result;
            }

            // 엑셀 파일에서 데이터 읽기
            var rows = MiniExcel.Query(filePath).ToList();

            // 헤더 행 찾기
            var headers = rows.FirstOrDefault();
            if (headers == null)
            {
                Log.Information("헤더 행을 찾을 수 없습니다.");
                return result;
            }

            // Dictionary로 헤더 맵핑
            var headerDict = ((IDictionary<string, object>)headers).ToDictionary(k => k.Value.ToString(), v => ((IDictionary<string, object>)headers).Keys.ToList().IndexOf(v.Key));

            foreach (var headerToFind in headersToFind)
            {
                // 헤더 값을 기준으로 해당 열(column) 인덱스 찾기
                if (headerDict.TryGetValue(headerToFind, out int snIndex))
                {
                    // 관련된 날짜 열(column) 찾기
                    string dateHeaderToFind = headerToFind + "Date";
                    if (headerDict.TryGetValue(dateHeaderToFind, out int dateIndex))
                    {
                        List<SnDate> columnData = new List<SnDate>();

                        foreach (var row in rows.Skip(1)) // 첫 번째 행은 헤더이므로 건너뜁니다
                        {
                            var rowDict = (IDictionary<string, object>)row;
                            var snValue = rowDict.Values.ElementAt(snIndex);
                            var dateValue = rowDict.Values.ElementAt(dateIndex);

                            if (snValue != null && dateValue != null && DateTime.TryParse(dateValue.ToString(), out DateTime date))
                            {
                                columnData.Add(new SnDate
                                {
                                    Sn = snValue.ToString() ?? "",
                                    Date = date
                                });
                            }
                        }

                        result.Add(headerToFind, columnData);
                    }
                    else
                    {
                        Log.Information($"관련 날짜 헤더 '{dateHeaderToFind}'를 찾을 수 없습니다.");
                    }
                }
                else
                {
                    Log.Information($"헤더 '{headerToFind}'를 찾을 수 없습니다.");
                }
            }

            return result;
        }
    }
}
