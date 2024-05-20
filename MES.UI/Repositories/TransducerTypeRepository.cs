using MES.UI.Context;
using MES.UI.Models;
using MES.UI.Repositories.Base;
using MES.UI.Repositories.interfaces;

namespace MES.UI.Repositories
{
    public class TransducerTypeRepository : RepositoryBase<TransducerType>, ITransducerTypeRepository
    {
        public TransducerTypeRepository(MESDbContext context) : base(context)
        {
        }
    }
}
