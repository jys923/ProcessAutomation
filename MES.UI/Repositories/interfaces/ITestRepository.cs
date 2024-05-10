using MES.UI.Models;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    public interface ITestRepository : IRepositoryBase<Test>
    {
        List<TestProbe> GetTestProbe();
        //List<TestProbe> GetTestProbe(DateTime? startDate, DateTime? endDate, int? categoryId, int? testTypeId,int? testerId, int? result, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn);
        //Task<List<TestProbe>> GetTestProbeAsync();
    }
}