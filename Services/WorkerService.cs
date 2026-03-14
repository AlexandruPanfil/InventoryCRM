using InventoryCRM.Data;
using InventoryCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryCRM.Services
{
    public class WorkerService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public WorkerService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        //For Read
        public async Task<List<Worker>> GetAllUsersAsync()
        {
            using var _context = _contextFactory.CreateDbContext();

            return await _context.Workers
                .Include(u => u.Deposit)
                .OrderBy(d => d.Workername).ToListAsync();
        }

        //For Read by Id
        public async Task<Worker> GetUsersAsync(Guid id)
        {
            using var _context = _contextFactory.CreateDbContext();

            return await _context.Workers.FindAsync(id);
        }

        //For Read
        public async Task<List<Worker>> FindUsersByNameAsync(string username)
        {
            using var _context = _contextFactory.CreateDbContext();

            return await _context.Workers
                .Where(u => u.Workername.Contains(username))
                .OrderBy(u => u.Workername)
                .ToListAsync();
        }

        //For Create
        public async Task<Worker> CreateUsersAsync(string workername, Guid? userId = null)
        {
            using var _context = _contextFactory.CreateDbContext();

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
            using var _context = _contextFactory.CreateDbContext();

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
            using var _context = _contextFactory.CreateDbContext();

            var worker = await _context.Workers.FindAsync(id);
            if (worker != null)
            {
                _context.Workers.Remove(worker);
                await _context.SaveChangesAsync();
            }
        }
    }

}

