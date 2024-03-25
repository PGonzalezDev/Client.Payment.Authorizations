using Client.Payments.Authorizations.Models;
using Client.Payments.Authorizations.Services.DTOs;

namespace Client.Payments.Authorizations.Services;

public interface IAuthorizationService
{
    Task<CreatedAuthorization> AddAuthorizationAsync(Authorization entity);
    Task<ConfirmAuthorizationResult> ConfirmAuthorizationAsync(ConfirmAuthorization confirmAuthorization);
}
