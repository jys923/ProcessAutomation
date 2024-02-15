using EFCoreSample.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSample.Data
{
    public class EFCoreSampleDbContext : DbContext
    {
        //public DbSet<User> Users { get; set; }
        //public DbSet<Postion> Postions { get; set; }
        public DbSet<Inspect> Inspects { get; set; }
        public DbSet<InspectType> InspectTypes { get; set; }
        public DbSet<Probe> Probes { get; set; }
        public DbSet<ProbeType> ProbeTypes { get; set; }
        //public DbSet<EntityBase> EntityBase { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            //string MSSQLConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AspnetCoreDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            //optionsBuilder.UseSqlServer(MSSQLConnectionString);
            string MariaDBConnectionString = @"Server=192.168.0.61; Port=3306; Database=sonocap_ems; Uid=root; Pwd=Endolfin12!@; SslMode=Preferred;";
            optionsBuilder.UseMySql(MariaDBConnectionString, ServerVersion.AutoDetect(MariaDBConnectionString));
        }
    }
}
