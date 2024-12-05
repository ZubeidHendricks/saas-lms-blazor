using SaasLMS.Shared.Models.Payment;
using SaasLMS.Shared.DTOs;

namespace SaasLMS.Server.Services.Payout;

public interface IPayoutService
{
    Task<InstructorEarning> CalculateEarningsAsync(Transaction transaction);
    Task<PayoutRequestDTO> CreatePayoutRequestAsync(Guid instructorId, PayoutRequestDTO request);
    Task<PayoutRequestDTO> GetPayoutRequestAsync(Guid requestId);
    Task<IEnumerable<PayoutRequestDTO>> GetInstructorPayoutRequestsAsync(Guid instructorId);
    Task<bool> ProcessPayoutRequestAsync(Guid requestId, bool approved, string? notes = null);
    Task<decimal> GetPendingEarningsAsync(Guid instructorId);
    Task<Dictionary<string, decimal>> GetEarningsSummaryAsync(Guid instructorId, DateTime startDate, DateTime endDate);
}