using Microsoft.EntityFrameworkCore;
using InventoryCRM.Models;
using InventoryCRM.Data;


namespace InventoryCRM.Services
{
    public class TodoService
    {
        private readonly ApplicationDbContext _context;
        public TodoService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<TodoItem>> GetAllTodosAsync()
        {
            return await _context.Todos.OrderByDescending(t => t.Date).ToListAsync();
        }
        public async Task<TodoItem> CreateTodoAsync(string title)
        {
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
            var task = await _context.Todos.FindAsync(id);
            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteTodoAsync(int id)
        {
            var task = await _context.Todos.FindAsync(id);
            if (task != null)
            {
                _context.Todos.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
    }
}
