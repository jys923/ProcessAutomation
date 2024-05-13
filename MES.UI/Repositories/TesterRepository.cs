using MES.UI.Models;
using MES.UI.Models.Context;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    public class TesterRepository : RepositoryBase<Tester> , ITesterRepository
    {
        public TesterRepository(MESDbContext context) : base(context)
        {
        }
    }
}
