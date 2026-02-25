using Microsoft.EntityFrameworkCore;
using InventoryCRM.Models;
using InventoryCRM.Data;
using InventoryCRM.Models.UnitModels;


namespace InventoryCRM.Services.UnitServices
{
    public class UnitService
    {
        private readonly ApplicationDbContext _context;

        public UnitService(ApplicationDbContext context)
        {
            _context = context;
        }

        // For getting all units
        public async Task<List<Unit>> GetAllUnitsAsync()
        {
            return await _context.Units.OrderBy(u => u.Name).ToListAsync();
        }

        // For getting all units name by quantity
        public async Task<List<Unit>> GetAllUnitsNameAndQuantityAsync()
        {
            var rawUnits =  _context.Units.OrderBy(u => u.Name).ToListAsync();
            var units = new List<Unit>();
            foreach (var unit in await rawUnits)
            {
                if (!units.Any(u => u.Name == unit.Name))
                {
                    units.Add(new Unit
                    {
                        Id = unit.Id,
                        Name = unit.Name,
                        Quantity = unit.Quantity
                    });
                }
                else
                {
                    units.Where(u => u.Name == unit.Name).FirstOrDefault()!.Quantity += unit.Quantity;
                }
            }
            return units;
        }

        // For getting deposits by unit name
        public async Task<List<Deposit>> GetDepositsByUnitNameAsync(string unitName)
        {
            if (string.IsNullOrWhiteSpace(unitName))
                return new List<Deposit>();

            var nameLower = unitName.Trim().ToLowerInvariant();

            // Return all units that match the name, and include their deposits
            return await _context.Deposits
                .Where(d => d.Unit.Any(u => u.Name.ToLower() == nameLower))
                .ToListAsync();
        }

        // For getting an units
        public async Task<Unit> GetUnitsAsync(Guid id)
        {
            return await _context.Units.FindAsync(id);
        }

        //Find units by Name
        public async Task<List<Unit>> FindUnitsAsync(string unitname)
        {
            return await _context.Units
                .Where(u => u.Name.Contains(unitname))
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        // For creating a new unit
        public async Task<Unit> CreateUnitsAsync(string name, int quantity, Guid depositId)
        {
            var unit = new Unit
            {
                Name = name,
                Quantity = quantity,
                DepositId = depositId
            };
            _context.Units.Add(unit);
            await _context.SaveChangesAsync();
            return unit;
        }

        // For updating an existing unit
        public async Task<Unit> UpdateUnitAsync(Guid id, string name, int quantity)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit != null)
            {
                unit.Name = name;
                unit.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
            return unit!;
        }

        // For deleting an existing unit
        public async Task DeleteUnitAsync(Guid id)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit != null)
            {
                _context.Units.Remove(unit);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Deposit>> GetDepositIdAsync()
        {
            return await _context.Deposits
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<bool> IsUnitInDepositAsync(Guid unitId, Guid depositID)
        {
            var unit = await _context.Units.FindAsync(unitId);
            if (unit != null && unit.DepositId == depositID)
            {
                return true;
            }
            return false;
        }
    }
}
