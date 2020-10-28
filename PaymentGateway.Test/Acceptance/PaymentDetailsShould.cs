using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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

namespace PaymentGateway.Testing.Acceptance
{
    [TestFixture]
    public sealed class PaymentDetailsShould
    {
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
        }

        [Test]
        public async Task ReturnPaymentDetails()
        {
            //Arrange
            var controller = new PaymentDetailsController(_paymentService);
            var paymentId = Guid.NewGuid();
            var bankResponseId = Guid.NewGuid();
            var paymentResult = new PaymentDetails(){Id = paymentId, BankResponseId = bankResponseId, PaymentMethod = new PaymentMethod()
                {
                    CardExpiry = "12/22",
                    CardNumber = "1000200030004000",
                    Cvv = "000"
                }
            };
            await _dbRepository.AddNewItem(paymentResult);
            
            //Act
            var response = await controller.Get(bankResponseId);
            var result = response as OkObjectResult;
            var paymentResponse = (PaymentResponse)result.Value;
            //Assert
            result.Should().NotBeNull();
            paymentResponse.BankResponseId.Should().Be(bankResponseId);
        }

    }
}
