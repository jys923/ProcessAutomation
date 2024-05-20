using MES.UI.Context;
using MES.UI.Models;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    public class TestTypeRepository : RepositoryBase<TestType>, ITestTypeRepository
    {
        public TestTypeRepository(MESDbContext context) : base(context)
        {
        }
    }
}
