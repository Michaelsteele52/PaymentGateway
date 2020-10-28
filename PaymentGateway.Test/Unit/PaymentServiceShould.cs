using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
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
        private TestHelper TestHelper;

        [SetUp]
        public void SetUp()
        {
            var serviceProvider = TestServiceProvider.GetDatabaseContext();
            _paymentRepository = new PaymentRepository(serviceProvider.GetService<PaymentsContext>());
            _idempotencyRepository = new IdempotencyKeyRepository(serviceProvider.GetService<PaymentsContext>());
            _bankService = A.Fake<IBankService>();
            _paymentService = new PaymentService(_paymentRepository, _idempotencyRepository, _bankService);
            TestHelper = new TestHelper(_paymentRepository, _idempotencyRepository);
        }

        [Test]
        public async Task ReturnAResponseWithMaskedCardNumberAndCvv()
        {
            //Arrange
            var idempotencyKey = Guid.NewGuid();
            var bankPaymentId = Guid.NewGuid();
            var paymentRequest = new PaymentRequest()
            {
                Amount = 10,
                Currency = "USD",
                PaymentMethod = new PaymentMethod()
                {
                    CardExpiry = "12/22",
                    CardNumber = "1000200030004000",
                    Cvv = "000"
                }
            };
            A.CallTo(() => _bankService.InitiatePayment(paymentRequest)).Returns(new PaymentResponse()
            {
                BankResponseId = bankPaymentId,
                Status = "Successful"
            });

            //Act
            var response = await _paymentService.SubmitPaymentRequest(idempotencyKey, paymentRequest);

            //Assert
            response.Status.Should().Be("Successful");
            A.CallTo(() => _bankService.InitiatePayment(A<PaymentRequest>.Ignored)).MustHaveHappened();
            response.PaymentMethod.Cvv.Should().Be("***");
            response.PaymentMethod.CardNumber.Should().Be("************4000");
        }

        [Test]
        public async Task ReturnExistingPaymentWhenIdempotencyKeyAlreadyPersisted()
        {
            //Arrange
            var idempotencyKey = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var bankPaymentId = Guid.NewGuid();
            var paymentMethod = new PaymentMethod()
            {
                CardExpiry = "12/22",
                CardNumber = "1000200030004000",
                Cvv = "000"
            };
            var paymentDetails = new PaymentDetails()
            {
                Id = paymentId,
                Amount = 10,
                Currency = "USD",
                BankResponseId = bankPaymentId,
                PaymentMethod = paymentMethod
            };
            TestHelper.CreateExistingPayment(idempotencyKey, paymentDetails);
            var paymentRequest = new PaymentRequest()
            {
                Amount = 10,
                Currency = "USD",
                PaymentMethod = paymentMethod
            };
            A.CallTo(() => _bankService.InitiatePayment(paymentRequest)).Returns(new PaymentResponse()
            {
                BankResponseId = bankPaymentId,
                Status = "Successful"
            });

            //Act
            var response = await _paymentService.SubmitPaymentRequest(idempotencyKey, paymentRequest);

            //Assert
            response.BankResponseId.Should().Be(bankPaymentId);
        }
    }
}
