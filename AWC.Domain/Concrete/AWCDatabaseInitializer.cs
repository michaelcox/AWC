using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using AWC.Domain.Entities;

namespace AWC.Domain.Concrete
{
    public class AWCDatabase : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<UsState> UsStates { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<ClientNote> ClientNotes { get; set; }
        public DbSet<RequestedItem> RequestedItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .HasRequired(c => c.UsState)
                .WithMany(s => s.Clients)
                .HasForeignKey(c => c.StateCode);

            modelBuilder.Entity<Client>()
                .HasRequired(c => c.County)
                .WithMany(s => s.Clients)
                .HasForeignKey(c => c.CountyCode);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.RequestedItems)
                .WithRequired(i => i.Client);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.ClientNotes)
                .WithRequired(i => i.Client);
        }
    }

    public static class Initialize
    {
        public static void Init(string connectionString)
        {
            Database.DefaultConnectionFactory = new SqlConnectionFactory(connectionString);
            Database.SetInitializer(new AWCDatabaseInitializer());
        }
    }

    public class AWCDatabaseInitializer : DropCreateDatabaseIfModelChanges<AWCDatabase>
    {
        protected override void Seed(AWCDatabase context)
        {
            context.UsStates.Add(new UsState { StateCode = "MD", StateName = "Maryland"});
            context.UsStates.Add(new UsState { StateCode = "VA", StateName = "Virginia" });
            context.UsStates.Add(new UsState { StateCode = "DC", StateName = "District of Columbia" });

            context.Counties.Add(new County { CountyCode = "MC", CountyName = "Montgomery County" });
            context.Counties.Add(new County { CountyCode = "PG", CountyName = "Prince Georges County" });

            context.SaveChanges();
            base.Seed(context);
        }
    }
}