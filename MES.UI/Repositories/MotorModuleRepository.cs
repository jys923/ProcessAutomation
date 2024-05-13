using MES.UI.Models;
using MES.UI.Models.Context;
using MES.UI.Repositories.Base;

namespace MES.UI.Repositories
{
    public class MotorModuleRepository : RepositoryBase<MotorModule> , IMotorModuleRepository
    {
        public MotorModuleRepository(MESDbContext context) : base(context)
        {
        }
    }
}
