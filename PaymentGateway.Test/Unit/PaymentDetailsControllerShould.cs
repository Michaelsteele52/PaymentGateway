using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PaymentGateway.Controllers;
using PaymentGateway.Data;
using PaymentGateway.Library.Models;

namespace PaymentGateway.Testing.Unit
{
    [TestFixture]
    public sealed class PaymentDetailsControllerShould
    {
        private PaymentDetailsController _controller;
        private IDbRespository<PaymentDetails> _dbRepository;

        [SetUp]
        public void SetUp()
        {
            var serviceProvider = TestServiceProvider.GetDatabaseContext();
            _dbRepository = new PaymentRepository(serviceProvider.GetService<PaymentsContext>());
            _controller = new PaymentDetailsController(_dbRepository);
        }

        [Test]
        public async Task ReturnNotFoundIfPaymentDoesNotExist()
        {
            //Arrange
            var paymentDetailsResponse = new PaymentDetailsResponse();
            var paymentId = Guid.Empty;

            //Act
            var result = await _controller.Get(paymentId);
            var status = result as NotFoundResult;

            //Assert
            status.Should().NotBeNull();
        }
    }
}
