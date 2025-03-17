using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SonoCap.MES.Repositories.Context
{
    public class MESDbDesignTimeContextFactory : IDesignTimeDbContextFactory<MESDbContext>
    {
        public MESDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MESDbContext>();

            // 환경설정 파일을 읽어와서 연결 문자열을 설정
            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();
            //
            //var connectionString = configuration.GetConnectionString("MariaDBConnection");
            string connectionString = @"Server=192.168.0.7; Port=3306; Database=sonocap_mes; Uid=root; Pwd=Endolfin12!@;AllowLoadLocalInfile=true;";

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options => options.CommandTimeout(120));

            return new MESDbContext(optionsBuilder.Options);
        }
    }

}

