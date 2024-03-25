using System;

namespace Client.Payments.Authorizations.Models
{
    public class Authorization
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public Guid ClientId { get; set; }
        public Client Client { get; set; }
        public DateTime CreatedDate { get; set; }
        public int AuthorizationType { get; set; }
        public bool Approved { get; set; }
        public bool? Confirmed { get; set; }
        public Guid? ReversedAuthorizationId { get; set; }
    }
}
