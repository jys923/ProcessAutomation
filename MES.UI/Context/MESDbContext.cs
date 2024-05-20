//#define MIGRATION

using MES.UI.Models;
using Microsoft.EntityFrameworkCore;

namespace MES.UI.Context
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

#if MIGRATION

        public MESDbContext()
        {
        }

        public MESDbContext(DbContextOptions options) : base(options)
        {
        }

        public MESDbContext(DbContextOptions<MESDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(dispose: true); // Serilog를 LoggerFactory에 추가
                builder.AddFilter((category, level) =>
                    category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information); // EF Core의 로그를 필터링하여 Serilog에게 전달
            });
#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging(true);
#endif
            //optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            optionsBuilder.UseLoggerFactory(loggerFactory); // Serilog에 EF Core 로그 리디렉션
            optionsBuilder.UseLazyLoadingProxies(true);
            string MariaDBConnectionString = Properties.Settings.Default.MariaDBConnection ?? throw new InvalidOperationException("MariaDBConnection is null.");
            optionsBuilder.UseMySql(MariaDBConnectionString, ServerVersion.AutoDetect(MariaDBConnectionString));
        }
        
#else
        public MESDbContext(DbContextOptions<MESDbContext> options) : base(options)
        {
        }

#endif
    }
}
