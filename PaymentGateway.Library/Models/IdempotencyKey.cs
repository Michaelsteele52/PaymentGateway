using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Library.Models
{
    public sealed class IdempotencyKey
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public Guid BankPaymentId { get; set; }
    }
}
