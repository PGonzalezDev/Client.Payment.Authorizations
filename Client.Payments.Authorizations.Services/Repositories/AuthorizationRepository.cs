using Client.Payments.Authorizations.Context;
using Client.Payments.Authorizations.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Client.Payments.Authorizations.Services.Repositories;

public class AuthorizationRepository : IAuthorizationRepository
{
    private readonly ClientPaymentAuthorizationContext _context;

    public AuthorizationRepository(
        ClientPaymentAuthorizationContext context
    )
    {
        _context = context;
    }

    public async Task<Authorization> AddAsync(Authorization entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        await _context.Authorizations.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task UpdateAsync(Authorization entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        _context.Authorizations.Update(entity);
        await _context.SaveChangesAsync(); ;
    }

    public async Task AddRangeAsync(IEnumerable<Authorization> entities)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        await _context.Authorizations.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<Authorization> entities)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        _context.Authorizations.UpdateRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid authorizationId)
    {
        ArgumentNullException.ThrowIfNull(authorizationId, nameof(authorizationId));

        bool exists = await _context.Authorizations.AnyAsync(auth => auth.Id == authorizationId);

        return exists;
    }

    public async Task<Authorization> FirstOrDefaultAsync(Expression<Func<Authorization, bool>> predicate)
        => await _context.Authorizations.FirstOrDefaultAsync(predicate);

    public async Task<Authorization> ConfirmAuthorizationAsync(Guid authorizationId, bool confirm)
    {
        ArgumentNullException.ThrowIfNull(authorizationId, nameof(authorizationId));

        Authorization entity = await _context.Authorizations.FirstOrDefaultAsync(auth => auth.Id == authorizationId);

        if (entity is null) { throw new NullReferenceException($"Authorization Id: {authorizationId} not found."); }

        entity.Confirmed = confirm;

        _context.Authorizations.Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<IEnumerable<Authorization>> GetDailyAuthorizationsToConfirmAsync(Guid clientId)
    {
        var authorizations = _context.Authorizations
            .Where(
                auth => auth.ClientId == clientId
                    && auth.CreatedDate >= DateTime.Today
                    && auth.CreatedDate <= DateTime.Today.AddDays(1)
                    && auth.Approved
                    && auth.Confirmed == null
            );

        return authorizations;
    }
}
