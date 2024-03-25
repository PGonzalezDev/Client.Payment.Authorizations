using Client.Payments.Authorizations.Models;
using System.Linq.Expressions;

namespace Client.Payments.Authorizations.Services.Repositories
{
    public interface IClientRepository
    {
        Task<Models.Client> GetClientByAuthorizationIdAsync(Guid authorizationId);
        Task<IEnumerable<Models.Client>> FindAsync(Expression<Func<Models.Client, bool>> predicate);
        Task<IEnumerable<Guid>> GetClientIdsByAuthorizationMode(ClientAuthModeEnum authorizationMode);
    }
}
