using Client.Payments.Authorizations.Context;
using Client.Payments.Authorizations.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Client.Payments.Authorizations.Services.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ClientPaymentAuthorizationContext _context;

    public ClientRepository(ClientPaymentAuthorizationContext context)
    {
        _context = context;
    }

    public async Task<Models.Client> GetClientByAuthorizationIdAsync(Guid authorizationId)
    {
        var authorization = await _context.Authorizations
            .Include(a => a.Client)
            .FirstOrDefaultAsync(a => a.Id == authorizationId);

        return authorization.Client;
    }

    public async Task<IEnumerable<Models.Client>> FindAsync(Expression<Func<Models.Client, bool>> predicate)
        => _context.Clients
                .Where(predicate)
                .AsEnumerable();

    public async Task<IEnumerable<Guid>> GetClientIdsByAuthorizationMode(ClientAuthModeEnum authorizationMode)
        => _context.Clients
                .Where(c => c.AuthorizationMode == (int)authorizationMode)
                .Select(c => c.Id)
                .AsEnumerable();
}
