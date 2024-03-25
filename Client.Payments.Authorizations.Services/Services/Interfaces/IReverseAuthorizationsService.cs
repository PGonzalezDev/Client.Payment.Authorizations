namespace Client.Payments.Authorizations.Services;

public interface IReverseAuthorizationsService
{
    Task ReverseAuthorizationsProcessAsync();
    Task UnconfirmExpiredAuthorizations(IEnumerable<Models.Authorization> expiredAuth);
    Task ReverseExpiredAuthorizations(IEnumerable<Models.Authorization> expiredAuth);
}
