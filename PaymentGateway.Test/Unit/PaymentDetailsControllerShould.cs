using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PaymentGateway.Controllers;
using PaymentGateway.Data;
using PaymentGateway.Library.Models;
using PaymentGateway.Library.Services;
using PaymentGateway.Services;

namespace PaymentGateway.Testing.Unit
{
    [TestFixture]
    public sealed class PaymentDetailsControllerShould
    {
        private PaymentDetailsController _controller;
        private IDbRespository<PaymentDetails> _dbRepository;
        private IPaymentService _paymentService;
        private IDbRespository<IdempotencyKey> _idempotencyRespository;
        private IBankService _bankService;

        [SetUp]
        public void SetUp()
        {
            var serviceProvider = TestServiceProvider.GetDatabaseContext();
            _dbRepository = new PaymentRepository(serviceProvider.GetService<PaymentsContext>());
            _idempotencyRespository = new IdempotencyKeyRepository(serviceProvider.GetService<PaymentsContext>());
            _bankService = A.Fake<IBankService>();
            _paymentService = new PaymentService(_dbRepository, _idempotencyRespository, _bankService);
            _controller = new PaymentDetailsController(_paymentService);
        }

        [Test]
        public async Task ReturnNotFoundIfPaymentDoesNotExist()
        {
            //Arrange
            var bankResponseId = Guid.NewGuid();

            //Act
            var result = await _controller.Get(bankResponseId);
            var status = result as NotFoundResult;

            //Assert
            status.Should().NotBeNull();
        }
    }
}
