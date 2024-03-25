using Client.Payments.Authorizations.Context;
using Client.Payments.Authorizations.Models;

namespace Client.Payments.Authorizations.Services.Repositories;

public class ApprovedAuthorizationRepository : IApprovedAuthorizationRepository
{
    private readonly ClientPaymentAuthorizationContext _context;

    public ApprovedAuthorizationRepository(
        ClientPaymentAuthorizationContext context
    )
    {
        _context = context;
    }

    public void Add(ApprovedAuthorization entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        _context.Add(entity);
        _context.SaveChanges(true);
    }
}
