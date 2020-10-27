using System;

namespace PaymentGateway.Library.Models
{
    public sealed class PaymentResponse 
    {
        public Guid PaymentId { get; set; }
        public string PaymentStatus { get; set; }
        public int StatusCode { get; set; }
    }
}
