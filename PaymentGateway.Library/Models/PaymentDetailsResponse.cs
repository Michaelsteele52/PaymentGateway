using System;

namespace PaymentGateway.Library.Models
{
    public sealed class PaymentDetailsResponse
    {
        public Guid PaymentId { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public PaymentDetails PaymentDetails { get; set; }
    }
}
