using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;
using SonoCap.MES.Repositories.Interfaces;

namespace SonoCap.MES.Repositories
{
    public class TestCategoryRepository : RepositoryBase<TestCategory>, ITestCategoryRepository
    {
        public TestCategoryRepository(MESDbContextFactory contextFactory) : base(contextFactory)
        {
        }
    }
}
