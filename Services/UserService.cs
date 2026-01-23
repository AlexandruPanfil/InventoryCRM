using InventoryCRM.Data;
using InventoryCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryCRM.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        //For Read
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.AppUsers
                .Include(u => u.Deposit)
                .OrderBy(d => d.Username).ToListAsync();
        }

        //For Read by Id
        public async Task<User> GetUsersAsync(Guid id)
        {
            return await _context.AppUsers.FindAsync(id);
        }

        //For Read
        public async Task<List<User>> FindUsersByNameAsync(string username)
        {
            return await _context.AppUsers
                .Where(u => u.Username.Contains(username))
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        //For Create
        public async Task<User> CreateUsersAsync(string username)
        {
            var newUser = new User
            {
                Username = username
            };
            _context.AppUsers.Add(newUser);
            await _context.SaveChangesAsync();

            var newDeposit = new Deposit
            {
                Name = username,
                UserId = newUser.Id,
            };
            _context.Deposits.Add(newDeposit);
            await _context.SaveChangesAsync();

            return newUser;
        }

        //For Update
        public async Task<User> UpdateUsersAsync(Guid id, string username)
        {
            var user = await _context.AppUsers.FindAsync(id);
            if (user != null)
            {
                user.Username = username;
                await _context.SaveChangesAsync();
            }
            return user;
        }

        //For Delete
        public async Task DeleteUsersAsync(Guid id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            if (user != null)
            {
                _context.AppUsers.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }

}

