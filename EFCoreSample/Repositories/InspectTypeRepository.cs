using EFCoreSample.Data;
using EFCoreSample.Entities;
using EFCoreSample.Repositories.Base;

namespace EFCoreSample.Repositories
{
    internal class InspectTypeRepository : RepositoryBase<InspectType> , IInspectTypeRepository
    {
        public InspectTypeRepository(EFCoreSampleDbContext context) : base(context)
        {
        }
    }
}
