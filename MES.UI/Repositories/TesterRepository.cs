using MES.UI.Models;
using MES.UI.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace MES.UI.Repositories
{
    public class TesterRepository : RepositoryBase<Tester> , ITesterRepository
    {
        public TesterRepository(DbContext context) : base(context)
        {
        }
    }
}
