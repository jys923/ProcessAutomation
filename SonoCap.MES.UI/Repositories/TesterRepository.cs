using SonoCap.MES.UI.Context;
using SonoCap.MES.UI.Models;
using SonoCap.MES.UI.Repositories.Base;

namespace SonoCap.MES.UI.Repositories
{
    public class TesterRepository : RepositoryBase<Tester>, ITesterRepository
    {
        public TesterRepository(MESDbContext context) : base(context)
        {
        }
    }
}
