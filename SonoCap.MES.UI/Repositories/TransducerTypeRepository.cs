using SonoCap.MES.UI.Context;
using SonoCap.MES.UI.Models;
using SonoCap.MES.UI.Repositories.Base;
using SonoCap.MES.UI.Repositories.interfaces;

namespace SonoCap.MES.UI.Repositories
{
    public class TransducerTypeRepository : RepositoryBase<TransducerType>, ITransducerTypeRepository
    {
        public TransducerTypeRepository(MESDbContext context) : base(context)
        {
        }
    }
}
