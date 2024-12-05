using Microsoft.EntityFrameworkCore;
using SaasLMS.Server.Data;
using SaasLMS.Server.Repositories.Base;
using SaasLMS.Shared.Models.Payment;

namespace SaasLMS.Server.Repositories.Payment;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(Guid userId)
    {
        return await DbSet
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetCourseTransactionsAsync(Guid courseId)
    {
        return await DbSet
            .Where(t => t.CourseId == courseId && t.Status == PaymentStatus.Completed)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionByPaymentIntentAsync(string paymentIntentId)
    {
        return await DbSet
            .FirstOrDefaultAsync(t => t.PaymentIntentId == paymentIntentId);
    }

    public async Task<bool> ProcessRefundAsync(Guid transactionId, decimal amount, string reason)
    {
        var transaction = await DbSet.FindAsync(transactionId);
        if (transaction == null) return false;

        transaction.IsRefunded = true;
        transaction.RefundedAmount = amount;
        transaction.RefundReason = reason;
        transaction.RefundedAt = DateTime.UtcNow;
        transaction.Status = PaymentStatus.Refunded;

        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = DbSet.Where(t => t.Status == PaymentStatus.Completed && !t.IsRefunded);

        if (startDate.HasValue)
            query = query.Where(t => t.CreatedAt >= startDate);

        if (endDate.HasValue)
            query = query.Where(t => t.CreatedAt <= endDate);

        return await query.SumAsync(t => t.Amount);
    }

    public async Task<Dictionary<string, decimal>> GetRevenueByPeriodAsync(
        DateTime startDate, 
        DateTime endDate, 
        string groupBy = "month")
    {
        var query = DbSet
            .Where(t => t.Status == PaymentStatus.Completed && 
                       !t.IsRefunded &&
                       t.CreatedAt >= startDate && 
                       t.CreatedAt <= endDate);

        return groupBy.ToLower() switch
        {
            "day" => await query
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new
                {
                    Period = g.Key.ToString("yyyy-MM-dd"),
                    Revenue = g.Sum(t => t.Amount)
                })
                .ToDictionaryAsync(x => x.Period, x => x.Revenue),

            "week" => await query
                .GroupBy(t => EF.Functions.DateTrunc("week", t.CreatedAt))
                .Select(g => new
                {
                    Period = g.Key!.Value.ToString("yyyy-MM-dd"),
                    Revenue = g.Sum(t => t.Amount)
                })
                .ToDictionaryAsync(x => x.Period, x => x.Revenue),

            "month" => await query
                .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                .Select(g => new
                {
                    Period = $"{g.Key.Year}-{g.Key.Month:00}",
                    Revenue = g.Sum(t => t.Amount)
                })
                .ToDictionaryAsync(x => x.Period, x => x.Revenue),

            "year" => await query
                .GroupBy(t => t.CreatedAt.Year)
                .Select(g => new
                {
                    Period = g.Key.ToString(),
                    Revenue = g.Sum(t => t.Amount)
                })
                .ToDictionaryAsync(x => x.Period, x => x.Revenue),

            _ => throw new ArgumentException("Invalid groupBy parameter. Use: day, week, month, or year")
        };
    }
}