using MES.UI.Entities;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    public interface IProbeSNRepository : IRepositoryBase<TransducerModule>
    {
        IEnumerable<TransducerModuleView> GetProbeSN(int resultCnt);
    }
}