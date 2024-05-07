using MES.UI.Models;
using MES.UI.Repositories.Base;
using MES.UI.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace MES.UI.Repositories
{
    public class TestCategoryRepository : RepositoryBase<TestCategory>, ITestCategoryRepository
    {
        public TestCategoryRepository(DbContext context) : base(context)
        {
        }
    }
}
