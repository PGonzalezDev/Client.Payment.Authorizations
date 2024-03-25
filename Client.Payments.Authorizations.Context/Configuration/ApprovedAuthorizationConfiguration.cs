using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Client.Payments.Authorizations.Context.Configuration;

public class ApprovedAuthorizationConfiguration : IEntityTypeConfiguration<Models.ApprovedAuthorization>
{
    public void Configure(EntityTypeBuilder<Models.ApprovedAuthorization> builder)
    {
        builder.HasKey(aa => aa.Id);
        builder.Property(aa => aa.Id).ValueGeneratedOnAdd();
        builder.Property(aa => aa.CreatedDate);
        builder.Property(aa => aa.Amount);
        builder.Property(aa => aa.ClientId);
    }
}