using System;

namespace PaymentGateway.Library.Models
{
    public sealed class PaymentDetails
    {
        public Guid Id { get; set; }
        public Guid BankResponseId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PaymentDate => DateTime.UtcNow;
        public string Status { get; set; } = "Requested by merchant";
    }
}
