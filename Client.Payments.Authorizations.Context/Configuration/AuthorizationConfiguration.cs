using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Client.Payments.Authorizations.Context.Configuration;

public class AuthorizationConfiguration : IEntityTypeConfiguration<Models.Authorization>
{
    public void Configure(EntityTypeBuilder<Models.Authorization> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.Amount);
        builder.Property(a => a.ClientId);
        builder.Property(a => a.CreatedDate);
        builder.Property(a => a.AuthorizationType);
        builder.Property(a => a.Approved);
        builder.Property(a => a.Confirmed);
        builder.Property(a => a.ReversedAuthorizationId);

        builder.HasOne(a => a.Client)
            .WithMany(c => c.Authorizations)
            .HasForeignKey(a => a.ClientId);
    }
}
