using SonoCap.MES.UI.Context;
using SonoCap.MES.UI.Models;
using SonoCap.MES.UI.Repositories.Base;
using SonoCap.MES.UI.Repositories.interfaces;

namespace SonoCap.MES.UI.Repositories
{
    public class TestCategoryRepository : RepositoryBase<TestCategory>, ITestCategoryRepository
    {
        public TestCategoryRepository(MESDbContext context) : base(context)
        {
        }
    }
}
