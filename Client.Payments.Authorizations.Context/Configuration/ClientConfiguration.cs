using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Client.Payments.Authorizations.Context.Configuration;

public class ClientConfiguration : IEntityTypeConfiguration<Models.Client>
{
    public void Configure(EntityTypeBuilder<Models.Client> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Name);
        builder.Property(c => c.Email);
        builder.Property(c => c.AuthorizationMode);

        builder.HasMany(c => c.Authorizations)
            .WithOne(a => a.Client)
            .HasForeignKey(a => a.ClientId);
    }
}
