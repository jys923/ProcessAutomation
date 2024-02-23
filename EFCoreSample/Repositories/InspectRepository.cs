using EFCoreSample.Data;
using EFCoreSample.Entities;
using EFCoreSample.Repositories.Base;

namespace EFCoreSample.Repositories
{
    internal class InspectRepository : RepositoryBase<Inspect> , IInspectRepository
    {
        public InspectRepository(EFCoreSampleDbContext context) : base(context)
        {
        }
    }
}
