using MES.UI.Data;
using MES.UI.Entities;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    internal class TestRepository : RepositoryBase<Test> , ITestRepository
    {
        public TestRepository(MESDbContext context) : base(context)
        {
        }
    }
}
