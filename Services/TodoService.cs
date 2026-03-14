using Microsoft.EntityFrameworkCore;
using InventoryCRM.Models;
using InventoryCRM.Data;


namespace InventoryCRM.Services
{
    public class TodoService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        public TodoService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<List<TodoItem>> GetAllTodosAsync()
        {
            using var _context = _contextFactory.CreateDbContext();
            return await _context.Todos.OrderByDescending(t => t.Date).ToListAsync();
        }
        public async Task<TodoItem> CreateTodoAsync(string title)
        {
            using var _context = _contextFactory.CreateDbContext();

            var task = new TodoItem
            {
                Todo = title,
                IsCompleted = false,
                Date = DateTime.UtcNow
            };
            _context.Todos.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }
        public async Task ToggleTodoAsync(int id)
        {
            using var _context = _contextFactory.CreateDbContext();

            var task = await _context.Todos.FindAsync(id);
            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteTodoAsync(int id)
        {
            using var _context = _contextFactory.CreateDbContext();

            var task = await _context.Todos.FindAsync(id);
            if (task != null)
            {
                _context.Todos.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
    }
}
