using SaasLMS.Shared.Models.Payment;
using SaasLMS.Shared.DTOs;

namespace SaasLMS.Server.Services.Payment;

public interface IPaymentService
{
    Task<Transaction> ProcessPaymentAsync(PaymentRequest request);
    Task<Transaction> ProcessRefundAsync(RefundRequest request);
    Task<bool> ValidatePaymentAsync(string paymentIntentId);
    Task<PaymentMethodDTO> AddPaymentMethodAsync(Guid userId, PaymentMethodRequest request);
    Task<IEnumerable<PaymentMethodDTO>> GetUserPaymentMethodsAsync(Guid userId);
    Task<bool> RemovePaymentMethodAsync(Guid userId, string paymentMethodId);
    Task<TransactionDTO> GetTransactionAsync(string transactionId);
    Task<IEnumerable<TransactionDTO>> GetUserTransactionsAsync(Guid userId);
}