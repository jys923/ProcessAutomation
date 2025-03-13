using MiniExcelLibs;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Converts;
using SonoCap.MES.UI.Services.Interfaces;
using System.IO;

namespace SonoCap.MES.UI.Services
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
            string extension = System.IO.Path.GetExtension(filePath).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                return false;
            }

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var excel = MiniExcel.Query(stream);
                    return excel != null;
                }
            }
            catch (IOException ex)
            {
                // 파일이 사용 중일 때 IOException 발생
                Log.Error(ex, "파일이 현재 사용 중이어서 접근할 수 없습니다.");
                return false;
            }
            catch (Exception ex)
            {
                // 다른 예외 처리
                Log.Error(ex, "파일을 읽는 중에 오류가 발생했습니다.");
                return false;
            }
        }

        public Dictionary<string, List<SnDate>> ReadColumnsDataByHeaders(string filePath, List<string> headersToFind)
        {
            var result = new Dictionary<string, List<SnDate>>();

            if (!IsExcelFile(filePath))
            {
                Log.Error("유효한 Excel 파일이 아닙니다.");
                throw new ArgumentException("유효한 Excel 파일이 아닙니다.", nameof(filePath));
            }

            // 엑셀 파일에서 데이터 읽기
            var rows = MiniExcel.Query(filePath).ToList();

            // 헤더 행 찾기
            var headers = rows.FirstOrDefault();
            if (headers == null)
            {
                Log.Error("헤더 행을 찾을 수 없습니다.");
                throw new ArgumentException("헤더 행을 찾을 수 없습니다.", nameof(filePath));
            }

            // 헤더 행이 IDictionary로 변환 가능한지 확인
            if (!(headers is IDictionary<string, object> headerDict))
            {
                Log.Error("헤더 행을 IDictionary로 변환할 수 없습니다.");
                throw new ArgumentException("헤더 행을 IDictionary로 변환할 수 없습니다.", nameof(filePath));
            }

            // Dictionary로 헤더 맵핑
            var headerDictionary = headerDict.ToDictionary(
                k => k.Value?.ToString() ?? "", // null 체크 추가
                v => headerDict.Keys.ToList().IndexOf(v.Key)
            );

            foreach (var headerToFind in headersToFind)
            {
                // 헤더 값을 기준으로 해당 열(column) 인덱스 찾기
                if (headerDictionary.TryGetValue(headerToFind, out int snIndex))
                {
                    // 관련된 날짜 열(column) 찾기
                    string dateHeaderToFind = headerToFind + "Date";
                    if (headerDictionary.TryGetValue(dateHeaderToFind, out int dateIndex))
                    {
                        List<SnDate> columnData = new List<SnDate>();

                        foreach (var row in rows.Skip(1)) // 첫 번째 행은 헤더이므로 건너뜁니다
                        {
                            var rowDict = (IDictionary<string, object>)row;
                            var snValue = rowDict.Values.ElementAtOrDefault(snIndex);
                            var dateValue = rowDict.Values.ElementAtOrDefault(dateIndex);

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
                        throw new ArgumentException($"관련 날짜 헤더 '{dateHeaderToFind}'를 찾을 수 없습니다.");
                    }
                }
                else
                {
                    Log.Information($"헤더 '{headerToFind}'를 찾을 수 없습니다.");
                    throw new ArgumentException($"헤더 '{headerToFind}'를 찾을 수 없습니다.");
                }
            }

            return result;
        }

    }
}
