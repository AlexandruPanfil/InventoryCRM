using Microsoft.EntityFrameworkCore;
using InventoryCRM.Models.UnitModels;
using InventoryCRM.Data;
using InventoryCRM.Models;

namespace InventoryCRM.Services.UnitServices
{
    public class UnitAssignmentService
    {
        private readonly ApplicationDbContext _context;

        public UnitAssignmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // For getting all reserved units
        public async Task<List<UnitAssignment>> GetAllUnitsReservedAsync()
        {
            return await _context.UnitsAssignment
                .Include(u => u.Customer)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        // For getting all reserved units by customer
        public async Task<List<UnitAssignment>> GetUnitsReservedByCustomerAsync(Guid customerId)
        {
            return await _context.UnitsAssignment
                .Where(u => u.CustomerId == customerId)
                .Include(u => u.Customer)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        // For getting a specific reserved unit
        public async Task<UnitAssignment?> GetUnitReservedAsync(Guid id)
        {
            return await _context.UnitsAssignment
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // Find reserved units by name
        public async Task<List<UnitAssignment>> FindUnitsReservedAsync(string unitName)
        {
            return await _context.UnitsAssignment
                .Where(u => u.Name.Contains(unitName))
                .Include(u => u.Customer)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        // For creating a new reserved unit
        public async Task<UnitAssignment> CreateUnitReservedAsync(string name, int quantity, Guid customerId)
        {
            var unitReserved = new UnitAssignment
            {
                Name = name,
                Quantity = quantity,
                CustomerId = customerId
            };
            _context.UnitsAssignment.Add(unitReserved);
            await _context.SaveChangesAsync();
            return unitReserved;
        }

        // For updating an existing reserved unit
        public async Task<UnitAssignment?> UpdateUnitReservedAsync(Guid id, string name, int quantity, string? status = null)
        {
            var unitReserved = await _context.UnitsAssignment.FindAsync(id);
            if (unitReserved != null)
            {
                unitReserved.Name = name;
                unitReserved.Quantity = quantity;
                if (!string.IsNullOrWhiteSpace(status))
                {
                    unitReserved.SetStatus(status);
                }
                await _context.SaveChangesAsync();
            }
            return unitReserved;
        }

        // For deleting a reserved unit
        public async Task DeleteUnitReservedAsync(Guid id)
        {
            var unitReserved = await _context.UnitsAssignment.FindAsync(id);
            if (unitReserved != null)
            {
                _context.UnitsAssignment.Remove(unitReserved);
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
            var unitReserved = await _context.UnitsAssignment.FindAsync(unitId);
            if (unitReserved != null && unitReserved.CustomerId == customerId)
            {
                return true;
            }
            return false;
        }
    }
}

