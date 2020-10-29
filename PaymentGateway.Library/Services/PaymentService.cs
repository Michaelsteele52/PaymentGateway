using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PaymentGateway.Data;
using PaymentGateway.Library.Models;
using PaymentGateway.Services;

namespace PaymentGateway.Library.Services
{
    public sealed class PaymentService : IPaymentService
    {
        private readonly IDbRespository<PaymentDetails> _paymentsRepository;
        private readonly IDbRespository<IdempotencyKey> _idempotencyRepository;
        private readonly IBankService _bankService;

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

            var paymentId = Guid.NewGuid();
            await _idempotencyRepository.AddNewItem(new IdempotencyKey()
            {
                Id = idempotencyKey,
                PaymentId = paymentId
            });

            //Better done with a Mapper
            var payment = new PaymentDetails()
            {
                Amount = paymentRequest.Amount,
                Currency = paymentRequest.Currency,
                Id = paymentId,
                PaymentMethod = paymentRequest.PaymentMethod
            };

            //Want to add encryption
            await _paymentsRepository.AddNewItem(payment);

            var response = await _bankService.InitiatePayment(paymentRequest);

            payment.Status = response.Status;
            payment.BankResponseId = response.BankResponseId;
            await _paymentsRepository.UpdateItem(payment);

            return await GetExistingPaymentResponse(payment.Id);
        }

        public async Task<PaymentResponse> GetPaymentResponse(Guid bankResponseId)
        {
            var payment = await _paymentsRepository.GetItem(x => x.BankResponseId == bankResponseId);
            if (payment == null)
            {
                return null;
            }

            return await GetExistingPaymentResponse(payment.Id);
        }

        private async Task<PaymentResponse> GetExistingPaymentResponse(Guid paymentId)
        {
            var existingPayment = await _paymentsRepository.GetItem(x => x.Id == paymentId);
            MaskPaymentMethod(existingPayment.PaymentMethod);
            return PaymentDetailsToResponse(existingPayment);
        }

        //In place, could be refactored to use a Mapper
        private PaymentResponse PaymentDetailsToResponse(PaymentDetails paymentDetails)
        {

            return new PaymentResponse()
            {
                BankResponseId = paymentDetails.BankResponseId,
                Status = paymentDetails.Status,
                PaymentMethod = paymentDetails.PaymentMethod
            };
        }

        private void MaskPaymentMethod(PaymentMethod paymentMethod)
        {
            paymentMethod.CardNumber = Regex.Replace(paymentMethod.CardNumber, @"\b[0-9]{12}(?=[0-9]{4}\b)", "************");
            paymentMethod.Cvv = "***";
        }
    }
}
