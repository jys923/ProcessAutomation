using EFCoreSample.Data;
using EFCoreSample.Entities;
using EFCoreSample.Repositories.Base;

namespace EFCoreSample.Repositories
{
    internal class ProbeSNRepository : RepositoryBase<ProbeSN> , IProbeSNRepository
    {
        public ProbeSNRepository(EFCoreSampleDbContext context) : base(context)
        {
        }
    }
}
