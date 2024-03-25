using Client.Payments.Authorizations.Services.DTOs;

namespace Client.Payment.Authorizations.Requests.Authorization;

public class ConfirmAuthorizationRequest
{
    public Guid AuthorizationId { get; set; }
    public bool Confirm { get; set; }

    public ConfirmAuthorization ToDto()
        => new ConfirmAuthorization()
        {
            AuthorizationId = AuthorizationId,
            Confirm = Confirm
        };

}
