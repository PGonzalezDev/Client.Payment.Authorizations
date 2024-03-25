using Client.Payments.Authorizations.Models;

namespace Client.Payments.Authorizations.Services.Helper
{
    public class AuthorizationTypeHelper
    {
        public string GetAuthorizationTypeDescription(int authorizationType)
        {
            return ((AuthorizationTypeEnum)authorizationType) switch
            {
                AuthorizationTypeEnum.PAYMENTS => nameof(AuthorizationTypeEnum.PAYMENTS),
                AuthorizationTypeEnum.RETURNS => nameof(AuthorizationTypeEnum.RETURNS),
                AuthorizationTypeEnum.REVERSE => nameof(AuthorizationTypeEnum.REVERSE),
                _ => string.Empty
            };
        }
    }
}
