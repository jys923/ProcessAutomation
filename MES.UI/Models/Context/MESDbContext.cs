using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MES.UI.Models.Context
{
    public class MESDbContext : DbContext
    {
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestCategory> TestCategories { get; set; }
        public DbSet<TestType> TestTypes { get; set; }
        public DbSet<Tester> Testers { get; set; }
        public DbSet<TransducerModule> TransducerModules { get; set; }
        public DbSet<ProbeType> TransducerModuleTypes { get; set; }
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
            string MariaDBConnectionString = Properties.Settings.Default.MariaDBConnection;
            optionsBuilder.UseMySql(MariaDBConnectionString, ServerVersion.AutoDetect(MariaDBConnectionString));
        }
    }
}
