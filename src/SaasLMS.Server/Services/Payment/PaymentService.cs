using SaasLMS.Server.Repositories.Payment;
using SaasLMS.Shared.Models.Payment;
using SaasLMS.Shared.DTOs;
using Stripe;
using Stripe.Checkout;

namespace SaasLMS.Server.Services.Payment;

public class PaymentService : IPaymentService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        ITransactionRepository transactionRepository,
        IConfiguration configuration,
        ILogger<PaymentService> logger)
    {
        _transactionRepository = transactionRepository;
        _configuration = configuration;
        _logger = logger;

        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    public async Task<Transaction> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100), // Convert to cents
                Currency = "usd",
                PaymentMethod = request.PaymentToken,
                Confirm = true,
                ConfirmationMethod = "automatic",
                SetupFutureUsage = "off_session",
                Metadata = new Dictionary<string, string>
                {
                    { "userId", request.UserId.ToString() },
                    { "courseId", request.CourseId.ToString() }
                }
            });

            var transaction = new Transaction
            {
                UserId = request.UserId,
                PaymentMethod = request.PaymentMethod,
                PaymentProvider = "Stripe",
                TransactionId = paymentIntent.Id,
                Amount = request.Amount,
                Currency = "USD",
                Status = ConvertPaymentStatus(paymentIntent.Status),
                PaymentIntentId = paymentIntent.Id,
                PaymentMethodId = request.PaymentToken,
                Type = TransactionType.CourseEnrollment,
                CourseId = request.CourseId,
                CreatedAt = DateTime.UtcNow,
                CompletedAt = paymentIntent.Status == "succeeded" ? DateTime.UtcNow : null
            };

            await _transactionRepository.AddAsync(transaction);
            return transaction;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Error processing payment");
            var transaction = new Transaction
            {
                UserId = request.UserId,
                PaymentMethod = request.PaymentMethod,
                PaymentProvider = "Stripe",
                Amount = request.Amount,
                Currency = "USD",
                Status = PaymentStatus.Failed,
                FailureReason = ex.Message,
                Type = TransactionType.CourseEnrollment,
                CourseId = request.CourseId,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(transaction);
            throw new PaymentProcessingException("Payment processing failed", ex);
        }
    }

    public async Task<Transaction> ProcessRefundAsync(RefundRequest request)
    {
        try
        {
            var refundService = new RefundService();
            var refund = await refundService.CreateAsync(new RefundCreateOptions
            {
                PaymentIntent = request.PaymentIntentId,
                Amount = (long)(request.Amount * 100), // Convert to cents
                Reason = ConvertRefundReason(request.Reason)
            });

            var transaction = await _transactionRepository
                .GetTransactionByPaymentIntentAsync(request.PaymentIntentId);

            if (transaction == null)
            {
                throw new InvalidOperationException("Transaction not found");
            }

            transaction.IsRefunded = true;
            transaction.RefundedAmount = request.Amount;
            transaction.RefundReason = request.Reason;
            transaction.RefundedAt = DateTime.UtcNow;
            transaction.Status = PaymentStatus.Refunded;

            await _transactionRepository.UpdateAsync(transaction);
            return transaction;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Error processing refund");
            throw new PaymentProcessingException("Refund processing failed", ex);
        }
    }

    private PaymentStatus ConvertPaymentStatus(string stripeStatus)
    {
        return stripeStatus switch
        {
            "succeeded" => PaymentStatus.Completed,
            "processing" => PaymentStatus.Processing,
            "requires_payment_method" => PaymentStatus.Pending,
            _ => PaymentStatus.Failed
        };
    }

    private string ConvertRefundReason(string reason)
    {
        return reason.ToLower() switch
        {
            "duplicate" => "duplicate",
            "fraudulent" => "fraudulent",
            "requested_by_customer" => "requested_by_customer",
            _ => "other"
        };
    }

    // Continue with payment methods management...
}