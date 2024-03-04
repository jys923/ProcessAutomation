using EFCoreSample.Data;
using EFCoreSample.Entities;
using EFCoreSample.Repositories.Base;

namespace EFCoreSample.Repositories
{
    internal class TestTypeRepository : RepositoryBase<TestType> , ITestTypeRepository
    {
        public TestTypeRepository(EFCoreSampleDbContext context) : base(context)
        {
        }
    }
}
