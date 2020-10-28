using System;
using System.Collections.Generic;
using System.Text;
using PaymentGateway.Data;
using PaymentGateway.Library.Models;

namespace PaymentGateway.Testing
{
    public class TestHelper
    {
        private static IDbRespository<PaymentDetails> _paymentRespository;
        private static IDbRespository<IdempotencyKey> _idempotentencyRespository;

        public TestHelper(IDbRespository<PaymentDetails> paymentRespository, IDbRespository<IdempotencyKey> idempotentencyRespository)
        {
            _paymentRespository = paymentRespository;
            _idempotentencyRespository = idempotentencyRespository;
        }

        public static void CreateExistingPayment(Guid idemKey, PaymentDetails paymentDetails)
        {
            _idempotentencyRespository.AddNewItem(new IdempotencyKey()
            {
                Id = idemKey,
                PaymentId = paymentDetails.Id
            });
            _paymentRespository.AddNewItem(paymentDetails);
        }
    }
}
