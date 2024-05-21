using SonoCap.MES.UI.Context;
using SonoCap.MES.UI.Models;
using SonoCap.MES.UI.Repositories.Base;

namespace SonoCap.MES.UI.Repositories
{
    public class MotorModuleRepository : RepositoryBase<MotorModule>, IMotorModuleRepository
    {
        public MotorModuleRepository(MESDbContext context) : base(context)
        {
        }
    }
}
