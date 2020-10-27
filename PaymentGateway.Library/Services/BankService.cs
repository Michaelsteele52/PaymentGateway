using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PaymentGateway.Library.Models;
using PaymentGateway.Services;

namespace PaymentGateway.Library.Services
{
    public sealed class BankService : IBankService
    {
        private readonly HttpClient _client;

        public BankService(HttpClient client)
        {
            _client = client;
        }

        public async Task<PaymentResponse> InitiatePayment(PaymentRequest request)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var httpResponse = await _client.PostAsync(_client.BaseAddress, requestContent);

            var responseContent = await httpResponse.Content.ReadAsStreamAsync();
            var responseJson = await JsonSerializer.DeserializeAsync<PaymentResponse>(responseContent);

            return responseJson;
        }
    }
}