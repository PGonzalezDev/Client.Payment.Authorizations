namespace Authorizations.Reverser.Worker.Services
{
    public interface IClientPaymentAuthorizationService
    {
        Task<bool> ReverseAuthorizationPost();
    }
}
