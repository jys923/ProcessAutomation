using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;
using SonoCap.MES.Repositories.Context;
using SonoCap.MES.Repositories.Interfaces;

namespace SonoCap.MES.Repositories
{
    public class TransducerRepository : RepositoryBase<Transducer>, ITransducerRepository
    {
        public TransducerRepository(MESDbContextFactory contextFactory) : base(contextFactory)
        {
        }
    }
}
