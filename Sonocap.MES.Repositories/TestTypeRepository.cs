using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;
using SonoCap.MES.Repositories.Interfaces;

namespace SonoCap.MES.Repositories
{
    public class TestTypeRepository : RepositoryBase<TestType>, ITestTypeRepository
    {
        public TestTypeRepository(MESDbContextFactory contextFactory) : base(contextFactory)
        {
        }
    }
}
