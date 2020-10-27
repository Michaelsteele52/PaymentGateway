using System;
using System.Collections.Generic;
using System.Text;
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
using PaymentGateway.Services;

namespace PaymentGateway.Testing.Acceptance
{
    [TestFixture]
    public sealed class ProcessPaymentShould
    {
        private PaymentController _paymentController;
        private IDbRespository<PaymentDetails> _dbRepository;
        private IPaymentService _paymentService;

        [SetUp]
        public void SetUp()
        {
            var serviceProvider = TestServiceProvider.GetDatabaseContext();
            _dbRepository = new PaymentRepository(serviceProvider.GetService<PaymentsContext>());
            _paymentService = A.Fake<IPaymentService>();
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
                    CardExpiry = new DateTime(2020, 12, 01),
                    CardNumber = "1000 2000 3000 4000",
                    Cvv = "000"
                }
            };
            var paymentId = Guid.NewGuid();
            var idempotencyKey = Guid.NewGuid();

            var paymentResponse = new PaymentResponse()
            {
                PaymentId = paymentId,
                PaymentStatus = "Payment Successful"
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
                    CardExpiry = new DateTime(2020, 12, 01),
                    CardNumber = "1000 2000 3000 4000",
                    Cvv = "000"
                }
            };
            var paymentId = Guid.NewGuid();
            var idempotencyKey = Guid.NewGuid();

            var paymentResponse = new PaymentResponse()
            {
                PaymentId = paymentId,
                PaymentStatus = "Payment Successful"
            };
            //Act
            await _paymentController.Post(idempotencyKey, newPaymentDetails);
            var response = await _paymentController.Post(idempotencyKey, newPaymentDetails);
            var result = response as OkObjectResult;
            var resultingPaymentId = (PaymentResponse)result?.Value;

            //Assert
            result.Should().NotBeNull();
            result.Value.IsSameOrEqualTo(paymentResponse);
            resultingPaymentId.PaymentId.Should().Be(paymentId);
        }
    }
}
