using EFCoreSample.Data;
using EFCoreSample.Entities;
using EFCoreSample.Repositories.Base;

namespace EFCoreSample.Repositories
{
    internal class TestRepository : RepositoryBase<Test> , ITestRepository
    {
        public TestRepository(EFCoreSampleDbContext context) : base(context)
        {
        }
    }
}
