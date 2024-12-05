using Microsoft.EntityFrameworkCore;
using SaasLMS.Server.Data;
using SaasLMS.Server.Repositories.Base;
using SaasLMS.Shared.Models.Payment;

namespace SaasLMS.Server.Repositories.Payment;

public class InstructorEarningRepository : Repository<InstructorEarning>, IInstructorEarningRepository
{
    public InstructorEarningRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<InstructorEarning>> GetInstructorEarningsAsync(Guid instructorId)
    {
        return await DbSet
            .Where(e => e.InstructorId == instructorId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalEarningsAsync(Guid instructorId)
    {
        return await DbSet
            .Where(e => e.InstructorId == instructorId)
            .SumAsync(e => e.EarnedAmount);
    }

    public async Task<decimal> GetUnpaidEarningsAsync(Guid instructorId)
    {
        return await DbSet
            .Where(e => e.InstructorId == instructorId && !e.IsPaid)
            .SumAsync(e => e.EarnedAmount);
    }

    public async Task<bool> MarkEarningsAsPaidAsync(Guid instructorId, Guid payoutRequestId)
    {
        var earnings = await DbSet
            .Where(e => e.InstructorId == instructorId && !e.IsPaid)
            .ToListAsync();

        if (!earnings.Any()) return false;

        foreach (var earning in earnings)
        {
            earning.IsPaid = true;
            earning.PaymentReference = payoutRequestId.ToString();
            earning.PaidAt = DateTime.UtcNow;
        }

        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<Dictionary<string, decimal>> GetEarningsByPeriodAsync(
        Guid instructorId,
        DateTime startDate,
        DateTime endDate)
    {
        return await DbSet
            .Where(e => e.InstructorId == instructorId &&
                       e.CreatedAt >= startDate &&
                       e.CreatedAt <= endDate)
            .GroupBy(e => new { e.CreatedAt.Year, e.CreatedAt.Month })
            .Select(g => new
            {
                Period = $"{g.Key.Year}-{g.Key.Month:00}",
                Earnings = g.Sum(e => e.EarnedAmount)
            })
            .ToDictionaryAsync(x => x.Period, x => x.Earnings);
    }
}