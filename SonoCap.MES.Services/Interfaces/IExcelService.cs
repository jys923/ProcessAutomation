using SonoCap.MES.Models;

namespace SonoCap.MES.Services.Interfaces
{
    public interface IExcelService
    {
        void ExportToExcel(IEnumerable<PTRView> data, string filePath);
        Dictionary<string, List<SnDate>> ReadColumnsDataByHeaders(string filePath, List<string> headersToFind);
    }
}