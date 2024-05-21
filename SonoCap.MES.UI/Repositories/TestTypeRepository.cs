using SonoCap.MES.UI.Context;
using SonoCap.MES.UI.Models;
using SonoCap.MES.UI.Repositories.Base;

namespace SonoCap.MES.UI.Repositories
{
    public class TestTypeRepository : RepositoryBase<TestType>, ITestTypeRepository
    {
        public TestTypeRepository(MESDbContext context) : base(context)
        {
        }
    }
}
