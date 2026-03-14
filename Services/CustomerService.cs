using InventoryCRM.Data;
using InventoryCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryCRM.Services
{
    public class CustomerService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public CustomerService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // Получить всех клиентов
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            using var _context = await _contextFactory.CreateDbContextAsync();

            return await _context.Customers
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // Получить клиента по Id
        public async Task<Customer?> GetCustomerByIdAsync(Guid id)
        {
            using var _context = await _contextFactory.CreateDbContextAsync();

            return await _context.Customers.FindAsync(id);
        }

        // Поиск клиентов по части имени (регистронезависимо)
        public async Task<List<Customer>> FindCustomerByNameAsync(string name)
        {
            using var _context = await _contextFactory.CreateDbContextAsync();

            if (string.IsNullOrWhiteSpace(name))
                return new List<Customer>();

            var pattern = $"%{name.Trim()}%";
            return await _context.Customers
                .Where(c => EF.Functions.Like(c.Name, pattern))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // Создать нового клиента
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            using var _context = await _contextFactory.CreateDbContextAsync();

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        // Обновить существующего клиента
        public async Task<Customer?> UpdateCustomerAsync(Guid id, Customer updated)
        {
            using var _context = await _contextFactory.CreateDbContextAsync();

            var existing = await _context.Customers.FindAsync(id);
            if (existing == null) return null;

            existing.IDNO = updated.IDNO;
            existing.Name = updated.Name;
            existing.Address = updated.Address;
            existing.PhoneNumber = updated.PhoneNumber;
            existing.Email = updated.Email;
            existing.Description = updated.Description;

            await _context.SaveChangesAsync();
            return existing;
        }

        // Удалить клиента
        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            using var _context = await _contextFactory.CreateDbContextAsync();

            var existing = await _context.Customers.FindAsync(id);
            if (existing == null) return false;

            _context.Customers.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
