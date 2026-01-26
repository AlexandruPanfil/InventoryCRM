using InventoryCRM.Data;
using InventoryCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryCRM.Services
{
    public class DepositService
    {
        private readonly ApplicationDbContext _context;
        public DepositService(ApplicationDbContext context)
        {
            _context = context;
        }

        //For Read All Deposits With Units
        public async Task<List<Deposit>> GetAllDepositsAsync()
        {
            return await _context.Deposits
                .Include(i => i.Unit)
                .OrderBy(d => d.Name).ToListAsync();
        }

        //For Read One Deposits With Units
        public async Task<List<Deposit>> GetOneDepositsAsync(Guid id)
        {
            return await _context.Deposits
                .Where(d => d.Id == id)
                .Include(i => i.Unit)
                .OrderBy(d => d.Name).ToListAsync();
        }

        //For Read by Id
        public async Task<Deposit> GetDepositsAsync(Guid id)
        {
            return await _context.Deposits.FindAsync(id);
        }

        //For Find by Name
        public async Task<List<Deposit>> FindDepositsAsync(string depositname)
        {
            return await _context.Deposits
                .Where(d => d.Name.Contains(depositname))
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        //For Create
        public async Task<Deposit> CreateDepositsAsync(string name)
        {
            var deposit = new Deposit
            {
                Name = name
            };
            _context.Deposits.Add(deposit);
            await _context.SaveChangesAsync();
            return deposit;
        }

        //For Update Name
        public async Task<Deposit> UpdateDepositsAsync(Guid id, string name)
        {
            var deposit = await _context.Deposits.FindAsync(id);
            if (deposit != null)
            {
                deposit.Name = name;
                await _context.SaveChangesAsync();
            }
            return deposit;
        }

        //For Update Units FRom Deposit Side
        public async Task<Deposit> UpdateDepositsUnitsAsync(Guid sourceID, Guid destinationID, ICollection<Unit> units)
        {
            var sourceDeposit = await _context.Deposits
                .Include(d => d.Unit)
                .FirstOrDefaultAsync(d => d.Id == sourceID);
            var destinationDeposit = await _context.Deposits
                .Include(d => d.Unit)
                .FirstOrDefaultAsync(d => d.Id == destinationID);

            if (sourceDeposit == null)
            {
                throw new InvalidOperationException("Source deposit not found.");
            }

            if (destinationDeposit == null) 
            { 
                throw new InvalidOperationException("Destination deposit not found.");
            }

            if (sourceDeposit.Unit.Count == 0)
            {
                throw new InvalidOperationException("Source deposit has no units to transfer.");
            }

            foreach (var unit in units)
            {
                var sourceUnitToTransfer = sourceDeposit.Unit.FirstOrDefault(u => u.Id == unit.Id && u.Name == unit.Name);
                var destinationUnitToTransfer = destinationDeposit.Unit.FirstOrDefault(u => u.Name == unit.Name);
                Unit newUnit = new Unit
                {
                    //Id = Guid.NewGuid(),
                    Name = unit.Name,
                    Quantity = unit.Quantity,
                    DepositId = destinationDeposit.Id
                };

                if (sourceUnitToTransfer.Quantity < unit.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient quantity in source deposit for unit {unit.Name}.");
                }

                sourceUnitToTransfer.Quantity -= unit.Quantity;

                if (sourceUnitToTransfer.Quantity == 0)
                {
                    _context.Deposits.Where(d => d.Id == sourceDeposit.Id)
                        .Include(d => d.Unit)
                        .First()
                        .Unit
                        .Remove(sourceUnitToTransfer);
                    _context.Units.Remove(sourceUnitToTransfer);
                }

                if (destinationUnitToTransfer != null)
                {
                    destinationUnitToTransfer.Quantity += unit.Quantity;
                    continue;
                }
                else
                {
                    destinationDeposit.Unit.Add(newUnit);
                }
                
            }
            await _context.SaveChangesAsync();
            return destinationDeposit;
        }

        //For Delete
        public async Task DeleteDepositsAsync(Guid id)
        {
            var deposit = await _context.Deposits.FindAsync(id);
            if (deposit != null)
            {
                _context.Deposits.Remove(deposit);
                await _context.SaveChangesAsync();
            }
        }
    }
}
