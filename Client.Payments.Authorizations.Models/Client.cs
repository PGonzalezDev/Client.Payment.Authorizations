using System;
using System.Collections.Generic;

namespace Client.Payments.Authorizations.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int AuthorizationMode { get; set; }

        public ICollection<Authorization> Authorizations { get; set; }
    }
}