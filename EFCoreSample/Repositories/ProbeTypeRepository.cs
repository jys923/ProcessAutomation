using EFCoreSample.Data;
using EFCoreSample.Entities;
using EFCoreSample.Repositories.Base;

namespace EFCoreSample.Repositories
{
    internal class ProbeTypeRepository : RepositoryBase<ProbeType> , IProbeTypeRepository
    {
        public ProbeTypeRepository(EFCoreSampleDbContext context) : base(context)
        {
        }
    }
}
