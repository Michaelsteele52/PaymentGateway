using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.Data;
using PaymentGateway.Library.Models;
using PaymentGateway.Services;

namespace PaymentGateway.Library.Services
{
    public sealed class PaymentService : IPaymentService
    {
        private IDbRespository<PaymentDetails> _paymentsRepository;
        private IDbRespository<IdempotencyKey> _idempotencyRepository;
        private IBankService _bankService;

        public PaymentService(IDbRespository<PaymentDetails> paymentsRepository, IDbRespository<IdempotencyKey> idempotencyRepository, IBankService bankService)
        {
            _paymentsRepository = paymentsRepository;
            _idempotencyRepository = idempotencyRepository;
            _bankService = bankService;
        }

        public async Task<PaymentResponse> SubmitPaymentRequest(Guid idempotencyKey, PaymentRequest paymentRequest)
        {
            var idemKey = await _idempotencyRepository.GetItem(x => x.Id == idempotencyKey);
            if (idemKey != null)
            {
                return await GetExistingPaymentResponse(idemKey.PaymentId);
            }
            var newPaymentId = Guid.NewGuid();
            
            var newPayment = new PaymentDetails()
            {
                Amount = paymentRequest.Amount,
                Currency = paymentRequest.Currency,
                Id = newPaymentId
            };

            await _paymentsRepository.AddNewItem(newPayment);

            var response = await _bankService.InitiatePayment(paymentRequest);

            //mask payment details


            newPayment.Status = response.PaymentStatus;
            newPayment.BankResponseId = response.PaymentId;
            await _paymentsRepository.UpdateItem(newPayment);

            return PaymentDetailsToResponse(newPayment);
        }

        private async Task<PaymentResponse> GetExistingPaymentResponse(Guid paymentId)
        {
            var existingPayment = await _paymentsRepository.GetItem(x => x.Id == paymentId);
            return PaymentDetailsToResponse(existingPayment);
        }

        private PaymentResponse PaymentDetailsToResponse(PaymentDetails paymentDetails)
        {
            return new PaymentResponse()
            {
                PaymentId = paymentDetails.Id,
                PaymentStatus = paymentDetails.Status
            };
        }
    }
}
