using Microsoft.EntityFrameworkCore;
using InventoryCRM.Models.UnitModels;
using InventoryCRM.Data;
using InventoryCRM.Models;

namespace InventoryCRM.Services.UnitServices
{
    public class UnitReservedService
    {
        private readonly ApplicationDbContext _context;

        public UnitReservedService(ApplicationDbContext context)
        {
            _context = context;
        }

        // For getting all reserved units
        public async Task<List<UnitReserved>> GetAllUnitsReservedAsync()
        {
            return await _context.UnitsReserved.OrderBy(u => u.Name).ToListAsync();
        }

        // For getting all reserved units by customer
        public async Task<List<UnitReserved>> GetUnitsReservedByCustomerAsync(Guid customerId)
        {
            return await _context.UnitsReserved
                .Where(u => u.CustomerId == customerId)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        // For getting a specific reserved unit
        public async Task<UnitReserved?> GetUnitReservedAsync(Guid id)
        {
            return await _context.UnitsReserved.FindAsync(id);
        }

        // Find reserved units by name
        public async Task<List<UnitReserved>> FindUnitsReservedAsync(string unitName)
        {
            return await _context.UnitsReserved
                .Where(u => u.Name.Contains(unitName))
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        // For creating a new reserved unit
        public async Task<UnitReserved> CreateUnitReservedAsync(string name, int quantity, Guid customerId)
        {
            var unitReserved = new UnitReserved
            {
                Name = name,
                Quantity = quantity,
                CustomerId = customerId
            };
            _context.UnitsReserved.Add(unitReserved);
            await _context.SaveChangesAsync();
            return unitReserved;
        }

        // For updating an existing reserved unit
        public async Task<UnitReserved?> UpdateUnitReservedAsync(Guid id, string name, int quantity)
        {
            var unitReserved = await _context.UnitsReserved.FindAsync(id);
            if (unitReserved != null)
            {
                unitReserved.Name = name;
                unitReserved.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
            return unitReserved;
        }

        // For deleting a reserved unit
        public async Task DeleteUnitReservedAsync(Guid id)
        {
            var unitReserved = await _context.UnitsReserved.FindAsync(id);
            if (unitReserved != null)
            {
                _context.UnitsReserved.Remove(unitReserved);
                await _context.SaveChangesAsync();
            }
        }

        // For getting all customers
        public async Task<List<Customer>> GetCustomersAsync()
        {
            return await _context.Customers
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // Check if reserved unit belongs to customer
        public async Task<bool> IsUnitReservedForCustomerAsync(Guid unitId, Guid customerId)
        {
            var unitReserved = await _context.UnitsReserved.FindAsync(unitId);
            if (unitReserved != null && unitReserved.CustomerId == customerId)
            {
                return true;
            }
            return false;
        }
    }
}
