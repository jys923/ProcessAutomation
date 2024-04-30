using MES.UI.Data;
using MES.UI.Entities;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    internal class TestTypeRepository : RepositoryBase<TestType> , ITestTypeRepository
    {
        public TestTypeRepository(MESDbContext context) : base(context)
        {
        }
    }
}
