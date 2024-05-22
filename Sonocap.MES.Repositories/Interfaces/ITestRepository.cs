using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;

namespace SonoCap.MES.Repositories.Interfaces
{
    public interface ITestRepository : IRepositoryBase<Test>
    {
        List<TestProbe> GetTestProbe();

        Task<List<TestProbe>> GetTestProbeAsync();

        List<TestProbe> GetTestProbe(DateTime? startDate, DateTime? endDate, int? categoryId, int? testTypeId, string? tester, int? pcId, int? result, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn);

        Task<List<TestProbe>> GetTestProbeAsync(DateTime? startDate, DateTime? endDate, int? categoryId, int? testTypeId, string? tester, int? pcId, int? result, int? dataFlagTest, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn, int? dataFlagProbe);
    }
}