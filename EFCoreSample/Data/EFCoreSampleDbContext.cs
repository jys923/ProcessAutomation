using EFCoreSample.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFCoreSample.Data
{
    public class EFCoreSampleDbContext : DbContext
    {
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestCategory> TestCategories { get; set; }
        public DbSet<TestType> TestTypes { get; set; }
        public DbSet<Tester> Testers { get; set; }
        public DbSet<TransducerModule> TransducerModules { get; set; }
        public DbSet<TransducerType> TransducerTypes { get; set; }
        public DbSet<MotorModule> MotorModules { get; set; }
        public DbSet<Probe> Probes { get; set; }

        //public DbSet<ProbeView> ProbeViews { get; set; }
        //public DbSet<ProbeSNView> ProbeSNViews { get; set; }
        //public DbSet<EntityBase> EntityBase { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
#if DEBUG
            // Enable sensitive data logging
            optionsBuilder.EnableSensitiveDataLogging(true);
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole())); 
#endif
            // Configure your database connection
            //string MSSQLConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AspnetCoreDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            //optionsBuilder.UseSqlServer(MSSQLConnectionString);
            string MariaDBConnectionString = @"Server=192.168.0.61; Port=3306; Database=sonocap_ems; Uid=root; Pwd=Endolfin12!@; SslMode=Preferred;";
            optionsBuilder.UseMySql(MariaDBConnectionString, ServerVersion.AutoDetect(MariaDBConnectionString));
        }
    }
}
