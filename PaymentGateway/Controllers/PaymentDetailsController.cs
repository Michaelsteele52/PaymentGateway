using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Data;
using PaymentGateway.Library.Models;
using PaymentGateway.Services;
using Serilog;

namespace PaymentGateway.Controllers
{
    [Route("payment-gateway/details")]
    [ApiController]
    public sealed class PaymentDetailsController : ControllerBase
    {
        private IPaymentService _paymentService;

        public PaymentDetailsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var paymentResponse = await _paymentService.GetPaymentResponse(id);

                if (paymentResponse == null)
                {
                    return NotFound();
                }

                return Ok(paymentResponse);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Requested payment with Id: {id}");
                throw;
            }
        }
    }
}
