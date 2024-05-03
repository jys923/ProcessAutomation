using MES.UI.Models;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    public interface IProbeSNRepository : IRepositoryBase<TransducerModule>
    {
        IEnumerable<TransducerModuleView> GetProbeSN();
        IEnumerable<TransducerModuleView> GetProbeSN2(int resultCnt);
    }
}