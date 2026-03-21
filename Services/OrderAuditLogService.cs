using InventoryCRM.Data;
using InventoryCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryCRM.Services
{
    public class OrderAuditLogService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public OrderAuditLogService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task LogAsync(
            Guid orderId,
            string action,
            string? oldValue,
            string? newValue,
            string? userId = null,
            string? userName = null)
        {
            using var ctx = _contextFactory.CreateDbContext();
            ctx.OrderAuditLogs.Add(new OrderAuditLog
            {
                OrderId = orderId,
                Action = action,
                OldValue = oldValue,
                NewValue = newValue,
                UserId = userId,
                UserName = userName,
                Timestamp = DateTime.UtcNow
            });
            await ctx.SaveChangesAsync();
        }

        public async Task<List<OrderAuditLog>> GetLogsForOrderAsync(Guid orderId)
        {
            using var ctx = _contextFactory.CreateDbContext();
            return await ctx.OrderAuditLogs
                .Where(l => l.OrderId == orderId)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }
    }
}