using System;
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
        public DbSet<Caseworker> Caseworkers { get; set; }
        public DbSet<PartneringOrg> PartneringOrgs { get; set; }

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

            modelBuilder.Entity<Client>()
                .HasOptional(c => c.Caseworker)
                .WithMany(c => c.Clients)
                .HasForeignKey(c => c.CaseworkerId);

            modelBuilder.Entity<Caseworker>()
                .HasRequired(c => c.ParneringOrg)
                .WithMany(p => p.Caseworkers)
                .HasForeignKey(c => c.PartneringOrgId);
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

            context.PartneringOrgs.Add(new PartneringOrg { OrganizationName = "Health and Human Services"});
            context.PartneringOrgs.Add(new PartneringOrg { OrganizationName = "Department of the Interior" });

            context.Clients.Add(new Client
                                    {
                                        FirstName = "Michael",
                                        LastName = "Cox",
                                        AddressLine1 = "123 Main Street",
                                        AddressLine2 = "Suite 103",
                                        City = "Worcester",
                                        StateCode = "MD",
                                        CountyCode = "MC",
                                        NumberOfAdults = 1,
                                        NumberOfChildren = 3,
                                        IsPreviousClient = false,
                                        IsReplacingFurniture = false,
                                        ReferredFrom = "I'm not sure where I heard of this organization.",
                                        CreatedDateTime = DateTime.Now,
                                        LastUpdatedDateTime = DateTime.Now
                                    });

            context.SaveChanges();
            base.Seed(context);
        }
    }
}