using Client.Payments.Authorizations.Models;

namespace Client.Payments.Authorizations.Services.Repositories
{
    public interface IApprovedAuthorizationRepository
    {
        void Add(ApprovedAuthorization entity);
    }
}
