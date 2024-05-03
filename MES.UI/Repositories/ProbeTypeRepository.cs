using MES.UI.Models;
using MES.UI.Models.Context;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    internal class ProbeTypeRepository : RepositoryBase<ProbeType> , IProbeTypeRepository
    {
        public ProbeTypeRepository(MESDbContext context) : base(context)
        {
        }
    }
}
