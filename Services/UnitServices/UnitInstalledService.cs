using Microsoft.EntityFrameworkCore;
using InventoryCRM.Models.UnitModels;
using InventoryCRM.Data;
using InventoryCRM.Models;

namespace InventoryCRM.Services.UnitServices
{
    public class UnitInstalledService
    {
        private readonly ApplicationDbContext _context;

        public UnitInstalledService(ApplicationDbContext context)
        {
            _context = context;
        }

        // For getting all installed units
        public async Task<List<UnitInstalled>> GetAllUnitsInstalledAsync()
        {
            return await _context.UnitsInstalled.OrderBy(u => u.Name).ToListAsync();
        }

        // For getting all installed units by customer
        public async Task<List<UnitInstalled>> GetUnitsInstalledByCustomerAsync(Guid customerId)
        {
            return await _context.UnitsInstalled
                .Where(u => u.CustomerId == customerId)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        // For getting a specific installed unit
        public async Task<UnitInstalled?> GetUnitInstalledAsync(Guid id)
        {
            return await _context.UnitsInstalled.FindAsync(id);
        }

        // Find installed units by name
        public async Task<List<UnitInstalled>> FindUnitsInstalledAsync(string unitName)
        {
            return await _context.UnitsInstalled
                .Where(u => u.Name.Contains(unitName))
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        // For creating a new installed unit
        public async Task<UnitInstalled> CreateUnitInstalledAsync(string name, int quantity, Guid customerId)
        {
            var unitInstalled = new UnitInstalled
            {
                Name = name,
                Quantity = quantity,
                CustomerId = customerId
            };
            _context.UnitsInstalled.Add(unitInstalled);
            await _context.SaveChangesAsync();
            return unitInstalled;
        }

        // For updating an existing installed unit
        public async Task<UnitInstalled?> UpdateUnitInstalledAsync(Guid id, string name, int quantity)
        {
            var unitInstalled = await _context.UnitsInstalled.FindAsync(id);
            if (unitInstalled != null)
            {
                unitInstalled.Name = name;
                unitInstalled.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
            return unitInstalled;
        }

        // For deleting an installed unit
        public async Task DeleteUnitInstalledAsync(Guid id)
        {
            var unitInstalled = await _context.UnitsInstalled.FindAsync(id);
            if (unitInstalled != null)
            {
                _context.UnitsInstalled.Remove(unitInstalled);
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

        // Check if installed unit belongs to customer
        public async Task<bool> IsUnitInstalledForCustomerAsync(Guid unitId, Guid customerId)
        {
            var unitInstalled = await _context.UnitsInstalled.FindAsync(unitId);
            if (unitInstalled != null && unitInstalled.CustomerId == customerId)
            {
                return true;
            }
            return false;
        }
    }
}
