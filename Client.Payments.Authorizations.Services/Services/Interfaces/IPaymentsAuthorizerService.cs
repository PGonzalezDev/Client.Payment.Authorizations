namespace Client.Payments.Authorizations.Services;

public interface IPaymentsAuthorizerService
{
    Task<bool> PaymentApproval(decimal amount);
}
