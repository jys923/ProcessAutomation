#define MIGRATION

using SonoCap.MES.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SonoCap.MES.Repositories.Context
{
    public class MESDbContext : DbContext
    {
        public DbSet<MotorModule> MotorModules { get; set; }
        public DbSet<Pc> Pcs { get; set; }
        public DbSet<Probe> Probes { get; set; }
        
        public DbSet<ProbeTestReport> ProbeTestReports { get; set; }
        public DbSet<ProbeTestResult> ProbeTestResults { get; set; }
        //public DbSet<ProbeTestResultView> ProbeTestResultViews { get; set; }
        public DbSet<PTRView> PTRViews { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestCategory> TestCategories { get; set; }
        public DbSet<Tester> Testers { get; set; }

        public DbSet<TestProbe> TestProbes { get; set; }
        public DbSet<TestType> TestTypes { get; set; }
        public DbSet<Transducer> Transducers { get; set; }
        public DbSet<TransducerModule> TransducerModules { get; set; }
        public DbSet<TransducerType> TransducerTypes { get; set; }

        public MESDbContext()
        {
        }

        public MESDbContext(DbContextOptions<MESDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ProbeTestResult 엔터티를 모델에서 제외합니다.
            modelBuilder.Ignore<ProbeTestReport>();
            modelBuilder.Ignore<ProbeTestResult>();
            //modelBuilder.Ignore<ProbeTestResultView>();
            modelBuilder.Ignore<TestProbe>();

            modelBuilder.Entity<PTRView>()
                .HasOne(ptrv => ptrv.Test01)
                .WithMany(t => t.PTRViewT01)
                .HasForeignKey(ptrv => ptrv.TestId01)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PTRView>()
                .HasOne(ptrv => ptrv.Test02)
                .WithMany(t => t.PTRViewT02)
                .HasForeignKey(ptrv => ptrv.TestId02)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PTRView>()
                .HasOne(ptrv => ptrv.Test03)
                .WithMany(t => t.PTRViewT03)
                .HasForeignKey(ptrv => ptrv.TestId03)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PTRView>()
                .HasOne(ptrv => ptrv.Test04)
                .WithMany(t => t.PTRViewT04)
                .HasForeignKey(ptrv => ptrv.TestId04)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PTRView>()
                .HasOne(ptrv => ptrv.Test05)
                .WithMany(t => t.PTRViewT05)
                .HasForeignKey(ptrv => ptrv.TestId05)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PTRView>()
                .HasOne(ptrv => ptrv.Test06)
                .WithMany(t => t.PTRViewT06)
                .HasForeignKey(ptrv => ptrv.TestId06)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PTRView>()
                .HasOne(ptrv => ptrv.Test07)
                .WithMany(t => t.PTRViewT07)
                .HasForeignKey(ptrv => ptrv.TestId07)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PTRView>()
                .HasOne(ptrv => ptrv.Test08)
                .WithMany(t => t.PTRViewT08)
                .HasForeignKey(ptrv => ptrv.TestId08)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PTRView>()
                .HasOne(ptrv => ptrv.Test09)
                .WithMany(t => t.PTRViewT09)
                .HasForeignKey(ptrv => ptrv.TestId09)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
#if MIGRATION
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
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            optionsBuilder.UseLoggerFactory(loggerFactory); // Serilog에 EF Core 로그 리디렉션
            optionsBuilder.UseLazyLoadingProxies(true);
            
            string MariaDBConnectionString = @"Server=192.168.0.61; Port=3306; Database=sonocap_mes_4; Uid=root; Pwd=Endolfin12!@;AllowLoadLocalInfile=true;";
            optionsBuilder.UseMySql(MariaDBConnectionString, ServerVersion.AutoDetect(MariaDBConnectionString), options => options.CommandTimeout(120));
        }
#endif
    }
}