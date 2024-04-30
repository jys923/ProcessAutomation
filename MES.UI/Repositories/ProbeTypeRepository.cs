using MES.UI.Data;
using MES.UI.Entities;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    internal class ProbeTypeRepository : RepositoryBase<TransducerModuleType> , IProbeTypeRepository
    {
        public ProbeTypeRepository(MESDbContext context) : base(context)
        {
        }
    }
}
