using EFCoreSample.Data;
using EFCoreSample.Entities;
using EFCoreSample.Repositories.Base;

namespace EFCoreSample.Repositories
{
    internal class TesterTypeRepository : RepositoryBase<Tester> , ITesterTypeRepository
    {
        public TesterTypeRepository(EFCoreSampleDbContext context) : base(context)
        {
        }
    }
}
