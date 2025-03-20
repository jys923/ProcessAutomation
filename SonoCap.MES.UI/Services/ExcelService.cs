using MiniExcelLibs;
using Serilog;
using SonoCap.MES.Models;
using SonoCap.MES.Models.Base;
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
            if (rows.Count == 0)
            {
                Log.Error("엑셀 파일이 비어 있습니다.");
                throw new ArgumentException("엑셀 파일이 비어 있습니다.", nameof(filePath));
            }

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

            // Dictionary로 헤더 매핑 (컬럼명 -> 컬럼 인덱스)
            var headerKeys = headerDict.Keys.ToList();
            var headerDictionary = headerDict.ToDictionary(
                k => k.Value?.ToString() ?? "",
                v => headerKeys.IndexOf(v.Key) // ✅ IndexOf()를 미리 리스트로 변환하여 최적화
            );

            foreach (var headerToFind in headersToFind)
            {
                if (!headerDictionary.TryGetValue(headerToFind, out int snIndex))
                {
                    Log.Warning($"헤더 '{headerToFind}'를 찾을 수 없습니다. 해당 데이터를 건너뜁니다.");
                    continue;
                }

                // 관련된 날짜 및 타입 열(column) 찾기
                string dateHeaderToFind = headerToFind + "Date";
                string typeHeaderToFind = headerToFind + "Type";

                bool hasDate = headerDictionary.TryGetValue(dateHeaderToFind, out int dateIndex);
                bool hasType = headerDictionary.TryGetValue(typeHeaderToFind, out int typeIndex);

                if (!hasDate)
                {
                    Log.Warning($"관련 날짜 헤더 '{dateHeaderToFind}'를 찾을 수 없습니다. 해당 데이터를 건너뜁니다.");
                    continue;
                }

                List<SnDate> columnData = new List<SnDate>();

                foreach (var row in rows.Skip(1))
                {
                    if (!(row is IDictionary<string, object> rowDict)) continue;

                    // ✅ TryGetValue() 사용으로 성능 최적화
                    rowDict.TryGetValue(headerKeys[snIndex], out var snValue);
                    rowDict.TryGetValue(headerKeys[dateIndex], out var dateValue);
                    string? typeValue = hasType && rowDict.TryGetValue(headerKeys[typeIndex], out var typeObj) ? typeObj?.ToString() : null; // ✅ Type이 없으면 null 유지

                    if (snValue != null && dateValue != null && DateTime.TryParse(dateValue.ToString(), out DateTime date))
                    {
                        columnData.Add(new SnDate
                        {
                            Sn = snValue.ToString()!,
                            Date = date,
                            Type = typeValue // ✅ Type이 없으면 null 유지
                        });
                    }
                }

                if (columnData.Count > 0)
                {
                    result[headerToFind] = columnData;
                }
            }

            return result;
        }

    }
}
