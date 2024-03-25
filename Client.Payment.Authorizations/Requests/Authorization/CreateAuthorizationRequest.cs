using Models = Client.Payments.Authorizations.Models;

namespace Client.Payment.Authorizations.Requests.Authorization
{
    public class CreateAuthorizationRequest
    {
        public decimal Amonut { get; set; }
        public Guid CliendId { get; set; }
        public int AuthorizationType { get; set; }

        public Models.Authorization ToEntity()
            => new Models.Authorization()
            {
                Amount = Amonut,
                ClientId = CliendId,
                AuthorizationType = (int)AuthorizationType
            };
    }
}
