using System.Threading.Tasks;
using PaymentGateway.Library.Models;

namespace PaymentGateway.Services
{
    public interface IBankService
    {
        Task<PaymentResponse> InitiatePayment(PaymentRequest request);
    }
}