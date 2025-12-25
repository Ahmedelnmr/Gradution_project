using Homy.Application.Contract_Service.ApiServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Homy.Application.Service.ApiServices
{
    public class PayPalService : IPayPalService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _baseUrl;

        public PayPalService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _clientId = configuration["PayPal:ClientId"] ?? throw new ArgumentNullException("PayPal:ClientId not configured");
            _clientSecret = configuration["PayPal:ClientSecret"] ?? throw new ArgumentNullException("PayPal:ClientSecret not configured");
            
            // Use sandbox URL for testing
            var mode = configuration["PayPal:Mode"] ?? "sandbox";
            _baseUrl = mode == "live" 
                ? "https://api-m.paypal.com" 
                : "https://api-m.sandbox.paypal.com";
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var authBytes = Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}");
            var authBase64 = Convert.ToBase64String(authBytes);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/v1/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authBase64);
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("access_token").GetString()!;
        }

        public async Task<(string orderId, string approvalUrl)> CreateOrderAsync(decimal amount, string currency, string description)
        {
            var accessToken = await GetAccessTokenAsync();

            var orderRequest = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        description = description,
                        amount = new
                        {
                            currency_code = currency,
                            value = amount.ToString("F2")
                        }
                    }
                },
                application_context = new
                {
                    return_url = "http://localhost:5235/api/subscriptions/payment-success",
                    cancel_url = "http://localhost:5235/api/subscriptions/payment-cancel"
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/v2/checkout/orders");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(
                JsonSerializer.Serialize(orderRequest), 
                Encoding.UTF8, 
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"PayPal CreateOrder failed: {json}");
            }

            using var doc = JsonDocument.Parse(json);
            var orderId = doc.RootElement.GetProperty("id").GetString()!;
            
            string approvalUrl = "";
            foreach (var link in doc.RootElement.GetProperty("links").EnumerateArray())
            {
                if (link.GetProperty("rel").GetString() == "approve")
                {
                    approvalUrl = link.GetProperty("href").GetString()!;
                    break;
                }
            }

            return (orderId, approvalUrl);
        }

        public async Task<(bool success, string? transactionId, string? payerId, string? errorMessage)> CaptureOrderAsync(string orderId)
        {
            var accessToken = await GetAccessTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/v2/checkout/orders/{orderId}/capture");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // Extract error message from PayPal response
                string errorMsg = "PayPal API Error";
                try
                {
                    using var errorDoc = JsonDocument.Parse(json);
                    if (errorDoc.RootElement.TryGetProperty("message", out var messageElement))
                        errorMsg = messageElement.GetString() ?? errorMsg;
                    if (errorDoc.RootElement.TryGetProperty("details", out var detailsElement) && detailsElement.GetArrayLength() > 0)
                        errorMsg += $" - {detailsElement[0].GetProperty("issue").GetString()}";
                }
                catch { errorMsg = json; }
                return (false, null, null, errorMsg);
            }

            using var doc = JsonDocument.Parse(json);
            
            var status = doc.RootElement.GetProperty("status").GetString();
            if (status != "COMPLETED")
            {
                return (false, null, null, $"Order status is {status}, not COMPLETED");
            }

            // Get transaction ID from captures
            string? transactionId = null;
            string? payerId = null;

            try
            {
                payerId = doc.RootElement.GetProperty("payer").GetProperty("payer_id").GetString();
                
                var purchaseUnits = doc.RootElement.GetProperty("purchase_units");
                foreach (var unit in purchaseUnits.EnumerateArray())
                {
                    var captures = unit.GetProperty("payments").GetProperty("captures");
                    foreach (var capture in captures.EnumerateArray())
                    {
                        transactionId = capture.GetProperty("id").GetString();
                        break;
                    }
                }
            }
            catch
            {
                // If we can't extract IDs, just use order ID as transaction ID
                transactionId = orderId;
            }

            return (true, transactionId, payerId, null);
        }
    }
}
