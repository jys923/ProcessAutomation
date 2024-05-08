using MES.UI.Models;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories.interfaces
{
    public interface IProbeRepository : IRepositoryBase<Probe>
    {
        List<ProbeTestResult> GetProbeSN();
        Task<List<ProbeTestResult>> GetProbeSNAsync();

        List<ProbeTestResult> GetProbeSN(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn);
        Task<List<ProbeTestResult>> GetProbeSNAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn);

        List<ProbeTestResult> GetProbeSNSql();
        Task<List<ProbeTestResult>> GetProbeSNSqlAsync();

        List<ProbeTestResult> GetProbeSNSql(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn);

        Task<List<ProbeTestResult>> GetProbeSNSqlAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn);

    }
}