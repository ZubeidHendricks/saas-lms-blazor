using SaasLMS.Shared.Models.Payment;

namespace SaasLMS.Server.Repositories.Payment;

public interface IInstructorEarningRepository : IRepository<InstructorEarning>
{
    Task<IEnumerable<InstructorEarning>> GetInstructorEarningsAsync(Guid instructorId);
    Task<decimal> GetTotalEarningsAsync(Guid instructorId);
    Task<decimal> GetUnpaidEarningsAsync(Guid instructorId);
    Task<bool> MarkEarningsAsPaidAsync(Guid instructorId, Guid payoutRequestId);
    Task<Dictionary<string, decimal>> GetEarningsByPeriodAsync(Guid instructorId, DateTime startDate, DateTime endDate);
}