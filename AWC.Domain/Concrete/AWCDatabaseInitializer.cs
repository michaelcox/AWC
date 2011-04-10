using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using AWC.Domain.Entities;

namespace AWC.Domain.Concrete
{
    public class AWCDatabase : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<UsState> UsStates { get; set; }
    }

    public static class Initialize
    {
        public static void Init(string connectionString)
        {
            Database.DefaultConnectionFactory = new SqlConnectionFactory(connectionString);
            Database.SetInitializer(new AWCDatabaseInitializer());
        }
    }

    public class AWCDatabaseInitializer : DropCreateDatabaseAlways<AWCDatabase>
    {
        protected override void Seed(AWCDatabase context)
        {
            context.UsStates.Add(new UsState { StateCode = "MD", StateName = "Maryland"});
            context.UsStates.Add(new UsState { StateCode = "VA", StateName = "Virginia" });
            context.UsStates.Add(new UsState { StateCode = "DC", StateName = "District of Columbia" });
            context.SaveChanges();
            base.Seed(context);
        }
    }
}