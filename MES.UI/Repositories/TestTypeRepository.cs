using MES.UI.Models;
using MES.UI.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace MES.UI.Repositories
{
    public class TestTypeRepository : RepositoryBase<TestType> , ITestTypeRepository
    {
        public TestTypeRepository(DbContext context) : base(context)
        {
        }
    }
}
