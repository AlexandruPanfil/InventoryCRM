using InventoryCRM.Components.Pages;
using InventoryCRM.Data;
using InventoryCRM.Models;
using InventoryCRM.Models.UnitModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace InventoryCRM.Services
{
    public class OrderService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public OrderService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        //For All Orders, Include Customer, UnitAssignment, Worker
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            using var _context = _contextFactory.CreateDbContext();

            return await _context.Orders
                .Include(o => o.Customers)
                .Include(o => o.UnitAssignment)
                .Include(o => o.Worker)
                .OrderByDescending(o => o.Status)
                .ToListAsync();
        }

        //For Single Order Find by ID
        public async Task<Order> GetOrderAsync(Guid id)
        {
            using var _context = _contextFactory.CreateDbContext();

            return await _context.Orders
                .Include(o => o.Customers)
                .Include(o => o.UnitAssignment)
                .Include(o => o.Worker)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        //For Finding Orders by Customer ID
        public async Task<List<Order>> GetOrdersByCustomerIdAsync(Guid customerId)
        {
            using var _context = _contextFactory.CreateDbContext();

            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.CustomersId == customerId)
                .Include(o => o.UnitAssignment)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        //For Finding Orders by Customer Name
        public async Task<List<Order>> FindOrdersByCustomerNameAsync(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                return new List<Order>();

            using var _context = _contextFactory.CreateDbContext();

            return await _context.Orders
                .Include(o => o.Customers)
                .Where(o => o.Customers.Name.Contains(customerName))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        //For Create a Order, Include UnitAssignment
        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            using var _context = _contextFactory.CreateDbContext();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        //For Update a Order, Include UnitAssignment
        public async Task<Order> UpdateOrderAsync(Guid id, 
            string? description = null, 
            string? status = null,
            ICollection<UnitAssignment>? selectedUnits = null,
            string? customerId = null,
            string? workerId = null)
        {

            using var _context = _contextFactory.CreateDbContext();

            var order = await _context.Orders
                .Include(o => o.UnitAssignment)
                .FirstOrDefaultAsync(u => u.Id == id);
                
            if (order != null)
            {
                order.UnitAssignment ??= new List<UnitAssignment>();

                var selectedUnitList = selectedUnits?.ToList() ?? new List<UnitAssignment>();

                if (!string.IsNullOrWhiteSpace(description))
                    order.Description = description;

                if (!string.IsNullOrWhiteSpace(status))
                    order.SetStatus(status);


                //Units Synchronization Logic:

                //Check ID of each unit from the UI, if it exists in the DB, update it, if not, add it.
                var selectedIds = selectedUnitList
                .Where(u => u.Id != Guid.Empty)
                .Select(u => u.Id)
                .ToHashSet();

                // Find units that are in the DB but not in the selected list (i.e., removed in the UI)
                var unitsToRemove = order.UnitAssignment!
                    .Where(u => !selectedIds.Contains(u.Id))
                    .ToList();

                // Remove units that are no longer selected in the UI
                foreach (var unit in unitsToRemove)
                {
                    order.UnitAssignment!.Remove(unit);
                    _context.UnitsAssignment.Remove(unit);
                }

                // Add or Update units from the UI
                foreach (var uiUnit in selectedUnitList)
                {
                    if (uiUnit.Id == Guid.Empty)
                    {
                        uiUnit.Id = Guid.NewGuid();
                    }

                    var dbUnit = order.UnitAssignment.FirstOrDefault(u => u.Id == uiUnit.Id);

                    if (dbUnit != null)
                    {
                        // Update existing unit values
                        dbUnit.Name = uiUnit.Name;
                        dbUnit.Quantity = uiUnit.Quantity;
                        dbUnit.CustomerId = order.CustomersId;
                    }
                    else
                    {
                        // It's a brand new unit (newly added in the UI)
                        // Ensure the ID is fresh if it's truly new, or just add it
                        order.UnitAssignment.Add(new UnitAssignment
                        {
                            Id = uiUnit.Id, 
                            Name = uiUnit.Name,
                            Quantity = uiUnit.Quantity,
                            CustomerId = order.CustomersId
                        });
                    }
                }

                if (customerId != null)
                {
                    Guid temp = Guid.Parse(customerId);
                    order.CustomersId = temp;
                }

                if (workerId != null) {
                    Guid temp = Guid.Parse(workerId);
                    order.WorkerId = temp;
                }

                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            return order!;
        }

        public async Task DeleteOrderAsync(Guid id)
        {
            using var _context = _contextFactory.CreateDbContext();

            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
