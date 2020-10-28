using System;

namespace PaymentGateway.Library.Models
{
    public sealed class PaymentResponse 
    {
        public Guid BankResponseId { get; set; }
        public string Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
    }
}
