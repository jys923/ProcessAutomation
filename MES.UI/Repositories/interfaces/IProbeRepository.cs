using MES.UI.Models;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories.interfaces
{
    public interface IProbeRepository : IRepositoryBase<Probe>
    {
        List<ProbeTestResult> GetProbeSN();
        Task<List<ProbeTestResult>> GetProbeSNAsync();
        
        List<ProbeTestResult> GetProbeSNSql();
        Task<List<ProbeTestResult>> GetProbeSNSqlAsync();
    }
}