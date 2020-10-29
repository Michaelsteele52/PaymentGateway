using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using PaymentGateway.Data;
using PaymentGateway.Library.Models;
using PaymentGateway.Services;
using Serilog;

namespace PaymentGateway.Controllers
{
    [Route("PaymentGateway/Process")]
    [ApiController]
    public sealed class PaymentController : ControllerBase
    {
        private IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Post([FromHeader(Name = "IdempotencyKey")] Guid idempotencyKey, [FromBody] PaymentRequest paymentRequest)
        {
            if (idempotencyKey.Equals(Guid.Empty))
                return BadRequest("Idempotency Key Header Required");
            if (paymentRequest == null)
                return UnprocessableEntity("Payment Details Missing");

            var response = await _paymentService.SubmitPaymentRequest(idempotencyKey, paymentRequest);

            return Ok(response);
        }

    }
}
