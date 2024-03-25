using Microsoft.EntityFrameworkCore;

namespace Client.Payments.Authorizations.Context;

public class ClientPaymentAuthorizationContext : DbContext
{
    public ClientPaymentAuthorizationContext(
        DbContextOptions<ClientPaymentAuthorizationContext> options
    ) : base(options)
    {
        SeedDataClient();
    }

    public DbSet<Models.Client> Clients { get; set; }
    public DbSet<Models.Authorization> Authorizations { get; set; }
    public DbSet<Models.ApprovedAuthorization> ApprovedAuthorizations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //TODO: Add 'modelBuilder.Entity<Models.Client>().HasData()' (Not working yet)
    }

    protected void SeedDataClient()
    {
        if (Clients != null && !Clients.Any())
        {
            Clients.AddRange(ClientSeedData.GetClientSeedData());
            SaveChanges();
        }
    }
}
