using System.Threading.Tasks;

namespace Homy.Application.Contract_Service.ApiServices
{
    public interface IPayPalService
    {
        /// <summary>
        /// Create a PayPal order for the specified amount
        /// </summary>
        /// <param name="amount">Amount to charge</param>
        /// <param name="currency">Currency code (USD, EUR, etc.)</param>
        /// <param name="description">Description of the purchase</param>
        /// <returns>Order ID and Approval URL</returns>
        Task<(string orderId, string approvalUrl)> CreateOrderAsync(decimal amount, string currency, string description);

        /// <summary>
        /// Capture an approved PayPal order
        /// </summary>
        /// <param name="orderId">The PayPal Order ID</param>
        /// <returns>Success status, transaction ID, payer ID, and error message</returns>
        Task<(bool success, string? transactionId, string? payerId, string? errorMessage)> CaptureOrderAsync(string orderId);
    }
}
