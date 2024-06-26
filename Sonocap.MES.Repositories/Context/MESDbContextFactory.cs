using Microsoft.EntityFrameworkCore;

namespace SonoCap.MES.Repositories.Context
{
    public class MESDbContextFactory : IDbContextFactory<MESDbContext>
    {
        public MESDbContext CreateDbContext()
        {
            //var optionsBuilder = new DbContextOptionsBuilder<MESDbContext>();
            //optionsBuilder.UseMySql("Your Connection String", ServerVersion.AutoDetect("Your Connection String"));

            //return new MESDbContext(optionsBuilder.Options);
            return new MESDbContext();
        }
    }
}
