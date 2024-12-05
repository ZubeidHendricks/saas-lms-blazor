using SaasLMS.Server.Repositories.Payment;
using SaasLMS.Shared.Models.Payment;
using SaasLMS.Shared.DTOs;

namespace SaasLMS.Server.Services.Payout;

public class PayoutService : IPayoutService
{
    private readonly IInstructorEarningRepository _earningRepository;
    private readonly IPayoutRequestRepository _payoutRequestRepository;
    private readonly IStripeService _stripeService;
    private readonly ITenantService _tenantService;
    private readonly ILogger<PayoutService> _logger;

    public PayoutService(
        IInstructorEarningRepository earningRepository,
        IPayoutRequestRepository payoutRequestRepository,
        IStripeService stripeService,
        ITenantService tenantService,
        ILogger<PayoutService> logger)
    {
        _earningRepository = earningRepository;
        _payoutRequestRepository = payoutRequestRepository;
        _stripeService = stripeService;
        _tenantService = tenantService;
        _logger = logger;
    }

    public async Task<InstructorEarning> CalculateEarningsAsync(Transaction transaction)
    {
        var course = await _courseRepository.GetByIdAsync(transaction.CourseId!.Value);
        if (course == null)
        {
            throw new InvalidOperationException("Course not found");
        }

        var tenantSettings = _tenantService.CurrentTenant.Settings;
        var instructorShare = tenantSettings.InstructorRevenueShare; // e.g., 0.70 for 70%

        var earning = new InstructorEarning
        {
            InstructorId = course.InstructorId,
            CourseId = course.Id,
            TransactionId = transaction.Id,
            CoursePrice = transaction.Amount,
            CommissionPercentage = instructorShare * 100,
            PlatformFee = transaction.Amount * (1 - instructorShare),
            EarnedAmount = transaction.Amount * instructorShare,
            TaxWithheld = CalculateTaxWithholding(transaction.Amount * instructorShare),
            CreatedAt = DateTime.UtcNow
        };

        return await _earningRepository.AddAsync(earning);
    }

    public async Task<PayoutRequestDTO> CreatePayoutRequestAsync(Guid instructorId, PayoutRequestDTO request)
    {
        var pendingEarnings = await _earningRepository.GetUnpaidEarningsAsync(instructorId);
        if (pendingEarnings < request.Amount)
        {
            throw new InvalidOperationException("Requested amount exceeds available earnings");
        }

        var payoutRequest = new PayoutRequest
        {
            InstructorId = instructorId,
            Amount = request.Amount,
            Currency = "USD",
            PaymentMethod = request.PaymentMethod,
            PaymentDetails = request.PaymentDetails,
            Status = PayoutStatus.Requested,
            RequestedAt = DateTime.UtcNow
        };

        // Get unpaid earnings up to the requested amount
        var earnings = await _earningRepository.FindAsync(e => 
            e.InstructorId == instructorId && 
            !e.IsPaid &&
            e.CreatedAt <= DateTime.UtcNow);

        decimal totalAdded = 0;
        foreach (var earning in earnings)
        {
            if (totalAdded + earning.EarnedAmount <= request.Amount)
            {
                payoutRequest.Earnings.Add(earning);
                totalAdded += earning.EarnedAmount;
            }
            else
            {
                break;
            }
        }

        await _payoutRequestRepository.AddAsync(payoutRequest);

        return MapToDTO(payoutRequest);
    }

    public async Task<bool> ProcessPayoutRequestAsync(Guid requestId, bool approved, string? notes = null)
    {
        var payoutRequest = await _payoutRequestRepository.GetByIdAsync(requestId);
        if (payoutRequest == null)
        {
            throw new InvalidOperationException("Payout request not found");
        }

        if (approved)
        {
            try
            {
                // Process payout via Stripe
                var transfer = await _stripeService.CreateTransferAsync(new StripeTransferRequest
                {
                    Amount = payoutRequest.Amount,
                    Currency = payoutRequest.Currency,
                    Destination = payoutRequest.PaymentDetails, // Stripe account ID
                    Description = $"Payout for request {payoutRequest.Id}"
                });

                payoutRequest.Status = PayoutStatus.Completed;
                payoutRequest.TransactionId = transfer.Id;
                payoutRequest.ProcessedAt = DateTime.UtcNow;

                // Mark earnings as paid
                await _earningRepository.MarkEarningsAsPaidAsync(
                    payoutRequest.InstructorId,
                    payoutRequest.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payout request {RequestId}", requestId);
                payoutRequest.Status = PayoutStatus.Failed;
                payoutRequest.ProcessingNotes = ex.Message;
                throw;
            }
        }
        else
        {
            payoutRequest.Status = PayoutStatus.Rejected;
            payoutRequest.ProcessingNotes = notes;
            payoutRequest.ProcessedAt = DateTime.UtcNow;
        }

        await _payoutRequestRepository.UpdateAsync(payoutRequest);
        return true;
    }

    public async Task<Dictionary<string, decimal>> GetEarningsSummaryAsync(
        Guid instructorId,
        DateTime startDate,
        DateTime endDate)
    {
        var earnings = await _earningRepository.GetEarningsByPeriodAsync(
            instructorId,
            startDate,
            endDate);

        var totalEarnings = await _earningRepository.GetTotalEarningsAsync(instructorId);
        var pendingEarnings = await _earningRepository.GetUnpaidEarningsAsync(instructorId);

        return new Dictionary<string, decimal>
        {
            { "totalEarnings", totalEarnings },
            { "pendingEarnings", pendingEarnings },
            { "paidEarnings", totalEarnings - pendingEarnings },
            { "periodEarnings", earnings.Sum(x => x.Value) }
        };
    }

    private decimal CalculateTaxWithholding(decimal amount)
    {
        // Implement tax calculation logic based on your requirements
        return 0; // For now, returning 0
    }

    private PayoutRequestDTO MapToDTO(PayoutRequest request)
    {
        return new PayoutRequestDTO
        {
            RequestId = request.Id,
            Amount = request.Amount,
            Status = request.Status.ToString(),
            PaymentMethod = request.PaymentMethod,
            RequestedAt = request.RequestedAt,
            ProcessedAt = request.ProcessedAt,
            ProcessingNotes = request.ProcessingNotes
        };
    }
}