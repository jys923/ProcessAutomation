using MES.UI.Data;
using MES.UI.Entities;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    internal class TesterTypeRepository : RepositoryBase<Tester> , ITesterTypeRepository
    {
        public TesterTypeRepository(MESDbContext context) : base(context)
        {
        }
    }
}
