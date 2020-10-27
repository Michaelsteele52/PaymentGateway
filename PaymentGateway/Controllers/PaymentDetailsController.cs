using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Data;
using PaymentGateway.Library.Models;
using Serilog;

namespace PaymentGateway.Controllers
{
    [Route("payment-gateway/details")]
    [ApiController]
    public sealed class PaymentDetailsController : ControllerBase
    {
        private IDbRespository<PaymentDetails> _paymentsRepository;

        public PaymentDetailsController(IDbRespository<PaymentDetails> paymentsRepository)
        {
            _paymentsRepository = paymentsRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var paymentDetails = await _paymentsRepository.GetItem(x => x.BankResponseId == id);

                if (paymentDetails == null)
                {
                    return NotFound();
                }

                return Ok(paymentDetails);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Requested payment with Id: {id}");
                throw;
            }
        }
    }
}
