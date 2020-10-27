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

namespace PaymentGateway.Testing.Acceptance
{
    [TestFixture]
    public sealed class PaymentDetailsShould
    {
        private IDbRespository<PaymentDetails> _dbRepository; 

        [SetUp]
        public void SetUp()
        {
            var serviceProvider = TestServiceProvider.GetDatabaseContext();
            _dbRepository = new PaymentRepository(serviceProvider.GetService<PaymentsContext>())
;       }

        [Test]
        public async Task ReturnPaymentDetails()
        {
            //Arrange
            var controller = new PaymentDetailsController(_dbRepository);
            var paymentId = Guid.NewGuid();
            var paymentResult = new PaymentDetails(){Id = paymentId};
            await _dbRepository.AddNewItem(paymentResult);
            
            //Act
            var response = await controller.Get(paymentId);
            var result = response as OkObjectResult;
            //Assert
            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(paymentResult);
        }

    }
}
