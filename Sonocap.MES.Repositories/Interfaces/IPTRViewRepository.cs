using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;

namespace SonoCap.MES.Repositories.Interfaces
{
    public interface IPTRViewRepository : IRepositoryBase<PTRView>
    {
        Task<List<PTRView>> GetProbeTestResultLinqAsync(DateTime? startDate, DateTime? endDate, string? probeSn, string? transducerModuleSn, string? transducerSn, string? motorModuleSn);

        public IQueryable<PTRView> GetPTRView(
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? probeSn = null,
            string? transducerModuleSn = null,
            string? transducerSn = null,
            string? motorModuleSn = null);
    }
}