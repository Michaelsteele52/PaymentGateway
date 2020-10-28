using System;
using System.Threading.Tasks;
using PaymentGateway.Library.Models;

namespace PaymentGateway.Services
{
    public interface IPaymentService
    {
        public Task<PaymentResponse> SubmitPaymentRequest(Guid idempotencyKey, PaymentRequest paymentRequest);
        public Task<PaymentResponse> GetPaymentResponse(Guid bankResponseId);
    }
}