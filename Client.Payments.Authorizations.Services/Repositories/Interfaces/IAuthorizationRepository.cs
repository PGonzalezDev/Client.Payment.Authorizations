using Client.Payments.Authorizations.Models;
using System.Linq.Expressions;

namespace Client.Payments.Authorizations.Services.Repositories;

public interface IAuthorizationRepository
{
    Task<Authorization> AddAsync(Authorization entity);
    Task UpdateAsync(Authorization entity);
    Task AddRangeAsync(IEnumerable<Authorization> entities);
    Task UpdateRangeAsync(IEnumerable<Authorization> entities);
    Task<bool> ExistsAsync(Guid authorizationId);
    Task<Authorization> FirstOrDefaultAsync(Expression<Func<Authorization, bool>> expression);
    Task<Authorization> ConfirmAuthorizationAsync(Guid authorizationId, bool confirmed);

    Task<IEnumerable<Authorization>> GetDailyAuthorizationsToConfirmAsync(Guid clientId);
}
