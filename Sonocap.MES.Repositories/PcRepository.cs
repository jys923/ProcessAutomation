using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;
using SonoCap.MES.Repositories.Interfaces;

namespace SonoCap.MES.Repositories
{
    public class PcRepository : RepositoryBase<Pc>, IPcRepository
    {
        public PcRepository(MESDbContextFactory contextFactory) : base(contextFactory)
        {
        }
    }
}
