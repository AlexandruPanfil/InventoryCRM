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

        //For Read
        public async Task<List<Deposit>> GetAllDepositsAsync()
        {
            return await _context.Deposits
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

        //For Update
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
