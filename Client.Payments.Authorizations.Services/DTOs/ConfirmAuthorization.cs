namespace Client.Payments.Authorizations.Services.DTOs;

public class ConfirmAuthorization
{
    public Guid AuthorizationId { get; set; }
    public bool Confirm { get; set; }
}
