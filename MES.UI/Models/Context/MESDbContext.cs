using Microsoft.EntityFrameworkCore;

namespace MES.UI.Models.Context
{
    public class MESDbContext : DbContext
    {
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestCategory> TestCategories { get; set; }
        public DbSet<TestType> TestTypes { get; set; }
        public DbSet<Tester> Testers { get; set; }
        public DbSet<TransducerModule> TransducerModules { get; set; }
        public DbSet<TransducerType> TransducerModuleTypes { get; set; }
        public DbSet<MotorModule> MotorModules { get; set; }
        public DbSet<Probe> Probes { get; set; }
        public DbSet<ProbeTestResult> ProbeTestResults { get; set; }
        public DbSet<TestProbe> TestProbes { get; set; }

        public MESDbContext(DbContextOptions<MESDbContext> options) : base(options)
        {
        }
    }
}
