using Microsoft.EntityFrameworkCore;
using InventoryCRM.Data;
using InventoryCRM.Models;

namespace InventoryCRM.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customers)
                .Include(o => o.UnitInstalled)
                .Include(o => o.UnitReserved)
                .Include(o => o.Worker)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> GetOrderAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Customers)
                .Include(o => o.UnitInstalled)
                .Include(o => o.UnitReserved)
                .Include(o => o.Worker)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetOrdersByCustomerIdAsync(Guid customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomersId == customerId)
                .Include(o => o.UnitInstalled)
                .Include(o => o.UnitReserved)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Order>> FindOrdersByCustomerNameAsync(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                return new List<Order>();

            return await _context.Orders
                .Include(o => o.Customers)
                .Where(o => o.Customers.Name.Contains(customerName))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateOrderAsync(Guid id, string? description = null, string? status = null, Guid? workerId = null)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                if (!string.IsNullOrWhiteSpace(description))
                    order.Description = description;

                if (!string.IsNullOrWhiteSpace(status))
                    order.SetStatus(status);

                order.WorkerId = workerId;
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            return order!;
        }

        public async Task DeleteOrderAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
