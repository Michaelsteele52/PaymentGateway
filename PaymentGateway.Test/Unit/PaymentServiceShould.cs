using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PaymentGateway.Data;
using PaymentGateway.Library.Models;
using PaymentGateway.Library.Services;
using PaymentGateway.Services;

namespace PaymentGateway.Testing.Unit
{
    [TestFixture]
    public sealed class PaymentServiceShould
    {
        private IPaymentService _paymentService;
        private IDbRespository<PaymentDetails> _paymentRepository;
        private IDbRespository<IdempotencyKey> _idempotencyRepository;
        private IBankService _bankService;

        [SetUp]
        public void SetUp()
        {
            var serviceProvider = TestServiceProvider.GetDatabaseContext();
            _paymentRepository = new PaymentRepository(serviceProvider.GetService<PaymentsContext>());
            _idempotencyRepository = new IdempotencyKeyRepository(serviceProvider.GetService<PaymentsContext>());
            _bankService = A.Fake<IBankService>();
            _paymentService = new PaymentService(_paymentRepository, _idempotencyRepository, _bankService);
        }

        [Test]
        public async Task GetPaymentIfItExists()
        {
            //Arrange
            var idempotencyKey = Guid.NewGuid();

            //Act
            var result = await _paymentService.SubmitPaymentRequest(idempotencyKey, new PaymentRequest());

            //Assert
        }

        [Test]
        public async Task ReturnAResponse()
        {
            //Arrange
            var idempotencyKey = Guid.NewGuid();
            var paymentRequest = new PaymentRequest()
            {
                Amount = 10,
                Currency = "USD",
                PaymentMethod = new PaymentMethod()
            };
            A.CallTo(() => _bankService.InitiatePayment(paymentRequest)).Returns(new PaymentResponse()
            {
                PaymentId = Guid.NewGuid(),
                PaymentStatus = "Approved",
                StatusCode = 1
            });

            //Act
            var response = await _paymentService.SubmitPaymentRequest(idempotencyKey, paymentRequest);

            //Assert
        }
    }
}
