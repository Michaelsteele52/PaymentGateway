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
using PaymentGateway.Services;
using PaymentGateway.Testing;

namespace PaymentGateway.Test
{
    [TestFixture]
    public class PaymentControllerShould
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
        public async Task ReturnBadRequestIfPaymentDetailsAreNull()
        {
            //Arrange
            var idempotencyKey = Guid.NewGuid();
            PaymentRequest newPaymentDetails = null;

            //Act
            var response = await _paymentController.Post(idempotencyKey, newPaymentDetails);
            var result = response as UnprocessableEntityObjectResult;

            //Assert
            result.Should().NotBeNull();
            result.Value.Should().Be("Payment Details Missing");
        }

        [Test]
        public async Task ReturnBadRequestIfIdempotentKeyIsNull()
        {
            //Arrange
            var paymentDetails = new PaymentRequest();
            var idempotencyKey = Guid.Empty;

            //Act
            var response = await _paymentController.Post(idempotencyKey, paymentDetails);
            var result = response as BadRequestObjectResult;
            
            //Assert
            result.Should().NotBeNull();
            result.Value.Should().Be("Idempotency Key Header Required");
        }

    }
}
