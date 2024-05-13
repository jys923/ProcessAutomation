using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MES.UI.Models.Context
{
    public class MESDbContext : DbContext
    {
        public DbSet<MotorModule> MotorModules { get; set; }
        public DbSet<Pc> Pcs { get; set; }
        public DbSet<Probe> Probes { get; set; }
        public DbSet<ProbeTestResult> ProbeTestResults { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestCategory> TestCategories { get; set; }
        public DbSet<Tester> Testers { get; set; }
        public DbSet<TestProbe> TestProbes { get; set; }
        public DbSet<TestType> TestTypes { get; set; }
        public DbSet<TransducerModule> TransducerModules { get; set; }
        public DbSet<TransducerType> TransducerTypes { get; set; }

        //public MESDbContext()
        //{
        //}

        public MESDbContext(DbContextOptions<MESDbContext> options) : base(options)
        {
        }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            base.OnConfiguring(optionsBuilder);
//#if DEBUG
//            optionsBuilder.EnableSensitiveDataLogging(true);
//            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
//#endif
//            optionsBuilder.UseLazyLoadingProxies(true);
//            string MariaDBConnectionString = MES.UI.Properties.Settings.Default.MariaDBConnection ?? throw new InvalidOperationException("MariaDBConnection is null.");
//            optionsBuilder.UseMySql(MariaDBConnectionString, ServerVersion.AutoDetect(MariaDBConnectionString));
//        }
    }
}
