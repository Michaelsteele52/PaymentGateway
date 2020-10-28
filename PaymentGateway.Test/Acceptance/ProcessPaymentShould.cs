using System;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PaymentGateway.Controllers;
using PaymentGateway.Data;
using PaymentGateway.Library.Models;
using PaymentGateway.Library.Services;
using PaymentGateway.Services;

namespace PaymentGateway.Testing.Acceptance
{
    [TestFixture]
    public sealed class ProcessPaymentShould
    {
        private PaymentController _paymentController;
        private IDbRespository<PaymentDetails> _dbRepository;
        private IDbRespository<IdempotencyKey> _idempotencyRespository;
        private IPaymentService _paymentService;
        private IBankService _bankService;

        [SetUp]
        public void SetUp()
        {
            var serviceProvider = TestServiceProvider.GetDatabaseContext();
            _dbRepository = new PaymentRepository(serviceProvider.GetService<PaymentsContext>());
            _idempotencyRespository = new IdempotencyKeyRepository(serviceProvider.GetService<PaymentsContext>());
            _bankService = A.Fake<IBankService>();
            _paymentService = new PaymentService(_dbRepository, _idempotencyRespository, _bankService);
            _paymentController = new PaymentController(_paymentService);
        }

        [Test]
        public async Task ReturnAcceptedWhenAPaymentIsSuccessfulWithThePaymentId()
        {
            //Arrange
            var newPaymentRequest = new PaymentRequest
            {
                Amount = 10,
                Currency = "GBP",
                PaymentMethod = new PaymentMethod()
                {
                    CardExpiry = "12/22",
                    CardNumber = "1000 2000 3000 4000",
                    Cvv = "000"
                }
            };
            var paymentId = Guid.NewGuid();
            var idempotencyKey = Guid.NewGuid();

            var paymentResponse = new PaymentResponse()
            {
                BankResponseId = paymentId,
                Status = "Successful"
            };
            //Act
            var result = await _paymentController.Post(idempotencyKey, newPaymentRequest);
            var response = result as OkObjectResult;

            //Assert
            response.Should().NotBeNull();
            response.Value.IsSameOrEqualTo(paymentResponse);
        }

        [Test]
        public async Task ReturnPaymentResultOfFirstRequestToDuplicateRequest()
        {
            //Arrange
            var newPaymentDetails = new PaymentRequest
            {
                Amount = 10,
                Currency = "GBP",
                PaymentMethod = new PaymentMethod()
                {
                    CardExpiry = "12/22",
                    CardNumber = "1000200030004000",
                    Cvv = "000"
                }
            };
            
            var idempotencyKey = Guid.NewGuid();

            await _paymentController.Post(idempotencyKey, newPaymentDetails);

            var payment = await _idempotencyRespository.GetItem(x => x.Id == idempotencyKey);
            var existingPaymentDetails = await _dbRepository.GetItem(x => x.Id == payment.PaymentId);

            var paymentResponse = new PaymentResponse()
            {
                BankResponseId = existingPaymentDetails.BankResponseId,
                Status = existingPaymentDetails.Status
            };

            //Act
            var response = await _paymentController.Post(idempotencyKey, newPaymentDetails);
            var result = response as OkObjectResult;
            var resultingPaymentId = (PaymentResponse)result?.Value;

            //Assert
            result.Should().NotBeNull();
            result.Value.IsSameOrEqualTo(paymentResponse);
            resultingPaymentId.BankResponseId.Should().Be(existingPaymentDetails.BankResponseId);
        }
    }
}
