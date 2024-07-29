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

        private IDictionary<string, List<object>> ImportExcel(string filePath, params string[] header)
        {
            List<dynamic> rows = MiniExcel.Query(filePath).ToList();
            Dictionary<string, List<object>> columnData = new Dictionary<string, List<object>>();

            if (rows.Any())
            {
                IDictionary<string, object> firstRowAsHeaders = (IDictionary<string, object>)rows.First();

                if (!header.Any())
                {
                    header = firstRowAsHeaders.Keys.ToArray();
                }

                foreach (var headerItem in header)
                {
                    if (firstRowAsHeaders.Values.Contains(headerItem))
                    {
                        Log.Information($"Header '{headerItem}' found");

                        List<object> columnValues = rows
                            .Skip(1)
                            .Where(row => ((IDictionary<string, object>)row).ContainsKey(headerItem))
                            .Select(row => ((IDictionary<string, object>)row)[headerItem])
                            .ToList();

                        columnData[headerItem] = columnValues;
                    }
                    else
                    {
                        Log.Information($"Header '{headerItem}' not found");
                    }
                }
            }
            else
            {
                Log.Information("No headers found in the file.");
            }

            return columnData;
        }

        public Dictionary<string, List<SnDate>> ReadColumnsDataByHeaders(string filePath, List<string> headersToFind)
        {
            var result = new Dictionary<string, List<SnDate>>();

            // 엑셀 파일에서 데이터 읽기
            var rows = MiniExcel.Query(filePath).ToList();

            // 헤더 행 찾기
            var headers = rows.FirstOrDefault();
            if (headers == null)
            {
                Console.WriteLine("헤더 행을 찾을 수 없습니다.");
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
                        Console.WriteLine($"관련 날짜 헤더 '{dateHeaderToFind}'를 찾을 수 없습니다.");
                    }
                }
                else
                {
                    Console.WriteLine($"헤더 '{headerToFind}'를 찾을 수 없습니다.");
                }
            }

            return result;
        }

        private IDictionary<string, List<SnDate>> ReadColumnsDataByHeaders3(string filePath, List<string> headersToFind)
        {
            var result = new Dictionary<string, List<SnDate>>();

            // 엑셀 파일에서 데이터 읽기
            var rows = MiniExcel.Query(filePath).ToList();

            // 헤더 행 찾기
            var headers = rows.FirstOrDefault();
            if (headers == null)
            {
                Console.WriteLine("헤더 행을 찾을 수 없습니다.");
                return result;
            }

            // 각 헤더 값에 대해 열(column) 인덱스 찾고 데이터 추출
            foreach (var headerToFind in headersToFind)
            {
                // 헤더 값을 기준으로 해당 열(column) 인덱스 찾기
                var headerIndex = ((IDictionary<string, object>)headers).Values.ToList().IndexOf(headerToFind);

                if (headerIndex != -1)
                {
                    // 해당 열(column) 데이터 추출
                    List<SnDate> columnData = new List<SnDate>();

                    foreach (var row in rows.Skip(1)) // 첫 번째 행은 헤더이므로 건너뜁니다
                    {
                        var cellValue = ((IDictionary<string, object>)row).Values.ElementAt(headerIndex);

                        // 셀 값이 null이 아닌 경우에만 처리
                        if (cellValue != null)
                        {
                            // Sn과 Date 값을 추출하여 SnDate 객체 생성
                            var snDate = new SnDate
                            {
                                Sn = headerToFind,
                                Date = DateTime.TryParse(cellValue.ToString(), out DateTime date) ? date : default
                            };
                            columnData.Add(snDate);
                        }
                    }

                    // 결과 딕셔너리에 추가
                    result.Add(headerToFind, columnData);
                }
                else
                {
                    Console.WriteLine($"헤더 '{headerToFind}'를 찾을 수 없습니다.");
                }
            }

            return result;
        }

        private IDictionary<string, List<object>> ReadColumnsDataByHeaders2(string filePath, List<string> headersToFind)
        {
            var result = new Dictionary<string, List<object>>();

            // 엑셀 파일에서 데이터 읽기
            var rows = MiniExcel.Query(filePath).ToList();

            // 첫 번째 행을 헤더로 간주
            var headers = rows.FirstOrDefault() as IDictionary<string, object>;
            if (headers == null)
            {
                Log.Information("헤더 행을 찾을 수 없습니다.");
                return result;
            }

            // 각 헤더 값에 대해 열(column) 인덱스 찾고 데이터 추출
            foreach (var headerToFind in headersToFind)
            {
                // 헤더 값을 기준으로 해당 열(column) 인덱스 찾기
                var headerKeys = headers.Values.ToList();
                var headerIndex = headerKeys.IndexOf(headerToFind);

                if (headerIndex != -1)
                {
                    // 해당 열(column) 데이터 추출
                    List<object> columnData = new List<object>();

                    foreach (var row in rows.Skip(1)) // 첫 번째 행은 헤더이므로 건너뜁니다
                    {
                        var rowDict = row as IDictionary<string, object>;
                        if (rowDict != null && rowDict.ContainsKey(headerToFind))
                        {
                            var cellValue = rowDict[headerToFind];

                            // 셀 값이 null이 아닌 경우에만 처리
                            if (cellValue != null)
                            {
                                columnData.Add(cellValue);
                            }
                            else
                            {
                                columnData.Add(string.Empty);
                            }
                        }
                    }

                    // 결과 딕셔너리에 추가
                    result.Add(headerToFind, columnData);
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
