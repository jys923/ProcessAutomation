using MES.UI.Models;
using MES.UI.Models.Context;
using MES.UI.Repositories.Base;
using MES.UI.Repositories.interfaces;

namespace MES.UI.Repositories
{
    public class PcRepository : RepositoryBase<Pc> , IPcRepository
    {
        public PcRepository(MESDbContext context) : base(context)
        {
        }
    }
}
