using InventoryCRM.Data;
using InventoryCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryCRM.Services
{
    public class WorkerService
    {
        private readonly ApplicationDbContext _context;

        public WorkerService(ApplicationDbContext context)
        {
            _context = context;
        }

        //For Read
        public async Task<List<Worker>> GetAllUsersAsync()
        {
            return await _context.Workers
                .Include(u => u.Deposit)
                .OrderBy(d => d.Workername).ToListAsync();
        }

        //For Read by Id
        public async Task<Worker> GetUsersAsync(Guid id)
        {
            return await _context.Workers.FindAsync(id);
        }

        //For Read
        public async Task<List<Worker>> FindUsersByNameAsync(string username)
        {
            return await _context.Workers
                .Where(u => u.Workername.Contains(username))
                .OrderBy(u => u.Workername)
                .ToListAsync();
        }

        //For Create
        public async Task<Worker> CreateUsersAsync(string workername, Guid? userId = null)
        {
            var newWorker = new Worker
            {
                Workername = workername
            };

            if (userId.HasValue)
            {
                newWorker.Id = userId.Value;
            }

            _context.Workers.Add(newWorker);
            await _context.SaveChangesAsync();

            var newDeposit = new Deposit
            {
                Name = workername,
                WorkerId = newWorker.Id,
            };
            _context.Deposits.Add(newDeposit);
            await _context.SaveChangesAsync();

            return newWorker;
        }

        //For Update
        public async Task<Worker> UpdateUsersAsync(Guid id, string username)
        {
            var worker = await _context.Workers.FindAsync(id);
            if (worker != null)
            {
                worker.Workername = username;
                await _context.SaveChangesAsync();
            }
            return worker;
        }

        //For Delete
        public async Task DeleteUsersAsync(Guid id)
        {
            var worker = await _context.Workers.FindAsync(id);
            if (worker != null)
            {
                _context.Workers.Remove(worker);
                await _context.SaveChangesAsync();
            }
        }
    }

}

