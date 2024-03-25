using Client.Payments.Authorizations.Models;
using Client.Payments.Authorizations.Services.Helper;

namespace Client.Payments.Authorizations.Services.DTOs;

public class CreatedAuthorization
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public Guid ClientId { get; set; }
    public DateTime CreatedDate { get; set; }
    public string AuthorizationType { get; set; }
    public bool Approved { get; set; }

    /// <summary>
    /// Empty Constructor
    /// </summary>
    public CreatedAuthorization() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="authorization"></param>
    public CreatedAuthorization(Authorization authorization)
    {
        Id = authorization.Id;
        Amount = authorization.Amount;
        ClientId = authorization.ClientId;
        CreatedDate = authorization.CreatedDate;
        AuthorizationType = new AuthorizationTypeHelper().GetAuthorizationTypeDescription(authorization.AuthorizationType);
        Approved = authorization.Approved;
    }
}
