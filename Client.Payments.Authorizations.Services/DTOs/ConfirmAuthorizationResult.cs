namespace Client.Payments.Authorizations.Services.DTOs;

public class ConfirmAuthorizationResult
{
    public Guid AuthorizationId { get; set; }
    public bool? Confirmed { get; set; }
    public ResultCodeEnum ResultCode { get; set; }
    public string? ErrorMsg { get; set; }
}
