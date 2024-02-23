using EFCoreSample.Data;
using EFCoreSample.Entities;
using EFCoreSample.Repositories.Base;

namespace EFCoreSample.Repositories
{
    internal class InspectorTypeRepository : RepositoryBase<InspectorType> , IInspectorTypeRepository
    {
        public InspectorTypeRepository(EFCoreSampleDbContext context) : base(context)
        {
        }
    }
}
