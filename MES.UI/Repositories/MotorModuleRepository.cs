using MES.UI.Models;
using MES.UI.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace MES.UI.Repositories
{
    public class MotorModuleRepository : RepositoryBase<MotorModule> , IMotorModuleRepository
    {
        public MotorModuleRepository(DbContext context) : base(context)
        {
        }
    }
}
