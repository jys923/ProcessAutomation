using Microsoft.EntityFrameworkCore;

namespace SonoCap.MES.Repositories.Context
{
    public class MESDbContextFactory : IDbContextFactory<MESDbContext>
    {
        public MESDbContext CreateDbContext()
        {
            //string conn = @"Server=192.168.0.61; Port=3306; Database=sonocap_mes; Uid=root; Pwd=Endolfin12!@;AllowLoadLocalInfile=true;";
            string conn = @"Server=localhost; Port=3306; Database=sonocap_mes; Uid=root; Pwd=Endolfin12!@;AllowLoadLocalInfile=true;";
            var optionsBuilder = new DbContextOptionsBuilder<MESDbContext>();
            optionsBuilder.UseMySql(conn, ServerVersion.AutoDetect(conn));

            return new MESDbContext(optionsBuilder.Options);
            //return new MESDbContext();
        }
    }
}
