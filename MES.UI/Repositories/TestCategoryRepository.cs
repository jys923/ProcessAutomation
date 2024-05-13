using MES.UI.Models;
using MES.UI.Models.Context;
using MES.UI.Repositories.Base;
using MES.UI.Repositories.interfaces;

namespace MES.UI.Repositories
{
    public class TestCategoryRepository : RepositoryBase<TestCategory>, ITestCategoryRepository
    {
        public TestCategoryRepository(MESDbContext context) : base(context)
        {
        }
    }
}
