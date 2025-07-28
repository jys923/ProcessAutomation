//#define MIGRATION

using SonoCap.MES.Models;
using Microsoft.EntityFrameworkCore;
using SonoCap.MES.Models.Base;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SonoCap.MES.Repositories.Context
{
    public class MESDbContext : DbContext
    {
        public DbSet<AppSettings> AppSettings { get; set; }
        public DbSet<MotorModule> MotorModules { get; set; }
        public DbSet<Pc> Pcs { get; set; }
        public DbSet<Probe> Probes { get; set; }
        public DbSet<PTRView> PTRViews { get; set; }
        public DbSet<SharedSeqNo> SharedSeqNos { get; set; }
        public DbSet<TestCategory> TestCategories { get; set; }
        public DbSet<Tester> Testers { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestType> TestTypes { get; set; }
        public DbSet<TransducerModule> TransducerModules { get; set; }
        public DbSet<Transducer> Transducers { get; set; }
        public DbSet<TransducerType> TransducerTypes { get; set; }
        
        public MESDbContext()
        {
        }

        public MESDbContext(DbContextOptions<MESDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AppSettings 엔티티의 ValueType 속성에 대한 변환 설정
            modelBuilder.Entity<AppSettings>()
                .Property(s => s.ValueType)
                .HasConversion(new EnumToStringConverter<SettingValueType>());
            // 또는 더 간결하게 .HasConversion<string>(); // 이 경우 EF Core가 적절한 컨버터를 찾아줌

            // SettingKey에 UNIQUE 인덱스 추가 (선택 사항이지만 권장)
            modelBuilder.Entity<AppSettings>()
                .HasIndex(s => s.SettingKey)
                .IsUnique();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ModelBase).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property("CreatedDate")
                        .HasDefaultValueSql("NOW()");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property("DataFlag")
                        .HasDefaultValue(1);
                }
            }

            modelBuilder.Entity<Test>()
                .ToTable(t => t.HasCheckConstraint("CK_Tests_Only_One_Not_Null",
               "(TransducerModuleId IS NOT NULL AND TransducerId IS NULL AND ProbeId IS NULL) OR " +
               "(TransducerModuleId IS NULL AND TransducerId IS NOT NULL AND ProbeId IS NULL) OR " +
               "(TransducerModuleId IS NULL AND TransducerId IS NULL AND ProbeId IS NOT NULL)"));

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

            //optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            optionsBuilder.UseLoggerFactory(loggerFactory); // Serilog에 EF Core 로그 리디렉션
            optionsBuilder.UseLazyLoadingProxies(true);

            string MariaDBConnectionString = string.Empty;
            MariaDBConnectionString = @"Server=192.168.0.61; Port=3306; Database=sonocap_mes; Uid=root; Pwd=Endolfin12!@;AllowLoadLocalInfile=true;";

            optionsBuilder.EnableSensitiveDataLogging(true);

            optionsBuilder.UseMySql(MariaDBConnectionString, ServerVersion.AutoDetect(MariaDBConnectionString), options => options.CommandTimeout(120));
        }
#endif
    }
}