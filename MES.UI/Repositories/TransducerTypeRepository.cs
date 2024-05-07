using MES.UI.Models;
using MES.UI.Repositories.Base;
using MES.UI.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace MES.UI.Repositories
{
    public class TransducerTypeRepository : RepositoryBase<TransducerType>, ITransducerTypeRepository
    {
        public TransducerTypeRepository(DbContext context) : base(context)
        {
        }
    }
}
