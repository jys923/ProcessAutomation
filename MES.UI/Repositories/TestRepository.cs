using MES.UI.Models;
using MES.UI.Models.Context;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    public class TestRepository : RepositoryBase<Test> , ITestRepository
    {
        public TestRepository(MESDbContext context) : base(context)
        {
        }
    }
}
