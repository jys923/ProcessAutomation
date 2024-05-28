using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;

namespace SonoCap.MES.Repositories.Interfaces
{
    public interface IPTRViewRepository : IRepositoryBase<PTRView>
    {
        Task<List<ProbeTestResult>> GetProbeTestResultLinqAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn);
        Task<List<ProbeTestResult>> GetProbeTestResultSqlAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn);
    }
}