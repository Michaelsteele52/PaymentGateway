using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Library.Models
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PaymentDate => DateTime.UtcNow;
    }
}
