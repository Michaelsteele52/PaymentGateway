using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Library.Models
{
    public sealed class PaymentMethod
    {
        public Guid PaymentMethodId { get; set; }
        public DateTime CardExpiry { get; set; }
        public string CardNumber { get; set; }
        public string Cvv { get; set; }
    }
}
