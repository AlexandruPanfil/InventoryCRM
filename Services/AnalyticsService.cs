using InventoryCRM.Data;
using InventoryCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryCRM.Services
{
    public class AnalyticsService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public AnalyticsService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // KPI: total open orders (New + Processing)
        public async Task<int> GetOpenOrdersCountAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Orders
                .CountAsync(o => o.Status == OrderStatus.New || o.Status == OrderStatus.Processing);
        }

        // KPI: orders whose schedule EndTime falls within the next 7 days
        public async Task<int> GetOrdersFinishingThisWeekAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            var now = DateTime.UtcNow;
            var week = now.AddDays(7);
            return await ctx.Orders
                .Include(o => o.Schedule)
                .CountAsync(o => o.Schedule != null
                               && o.Schedule.EndTime >= now
                               && o.Schedule.EndTime <= week
                               && o.Status != OrderStatus.Finished);
        }

        // KPI: workers whose personal deposit has zero units in stock
        public async Task<int> GetWorkersWithEmptyDepositAsync()
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Workers
                .Include(w => w.Deposit)
                    .ThenInclude(d => d.Unit)
                .CountAsync(w => w.Deposit != null
                               && (w.Deposit.Unit == null || w.Deposit.Unit.Count == 0));
        }

        // Chart: top-N most-consumed unit names (by total quantity across all deposits)
        public async Task<List<(string Name, int TotalQty)>> GetTopConsumedUnitsAsync(int top = 8)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Units
                .GroupBy(u => u.Name)
                .Select(g => new { Name = g.Key, Total = g.Sum(u => u.Quantity) })
                .OrderByDescending(x => x.Total)
                .Take(top)
                .Select(x => new ValueTuple<string, int>(x.Name, x.Total))
                .ToListAsync();
        }

        // Feed: last N order state changes (CreatedAt / UpdatedAt)
        public async Task<List<Order>> GetRecentOrderActivityAsync(int count = 10)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.Orders
                .Include(o => o.Customers)
                .Include(o => o.Worker)
                .OrderByDescending(o => o.UpdatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}