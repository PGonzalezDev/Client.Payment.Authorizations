using System;

namespace Client.Payments.Authorizations.Models
{
    public class ApprovedAuthorization
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Amount { get; set; }
        public Guid ClientId { get; set; }

        public ApprovedAuthorization() { }

        public ApprovedAuthorization(Authorization authorization)
        {
            if (authorization != null)
            {
                CreatedDate = authorization.CreatedDate;
                Amount = authorization.Amount;
                ClientId = authorization.ClientId;
            }
        }
    }
}
