using Client.Payments.Authorizations.Services.DTOs;

namespace Client.Payment.Authorizations.Responses
{
    public class ConfirmAuthorizationResponse
    {
        public Guid AuthorizationId { get; set; }
        public bool? Confirmed { get; set; }

        public ConfirmAuthorizationResponse() { }

        public ConfirmAuthorizationResponse(ConfirmAuthorizationResult result) 
        {
            ArgumentNullException.ThrowIfNull(result, nameof(result));

            AuthorizationId = result.AuthorizationId;
            Confirmed = result.Confirmed;
        }
    }
}
