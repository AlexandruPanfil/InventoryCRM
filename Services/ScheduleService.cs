using InventoryCRM.Data;
using InventoryCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryCRM.Services
{
    public class ScheduleService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public ScheduleService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Schedule>> GetAllSchedulesAsync()
        {
            using var _context = _contextFactory.CreateDbContext();
            return await _context.Schedules
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<Schedule?> GetScheduleAsync(Guid id)
        {
            using var _context = _contextFactory.CreateDbContext();
            return await _context.Schedules.FindAsync(id);
        }

        public async Task<List<Schedule>> GetSchedulesInRangeAsync(DateTime start, DateTime end)
        {
            using var _context = _contextFactory.CreateDbContext();
            return await _context.Schedules
                .Where(s => s.StartTime >= start && s.EndTime <= end)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<Schedule> CreateScheduleAsync(DateTime start, DateTime end)
        {
            if (end <= start) throw new ArgumentException("EndTime must be later than StartTime.");

            using var _context = _contextFactory.CreateDbContext();
            var schedule = new Schedule { StartTime = start, EndTime = end };
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<Schedule?> UpdateScheduleAsync(Guid id, DateTime start, DateTime end)
        {
            if (end <= start) throw new ArgumentException("EndTime must be later than StartTime.");

            using var _context = _contextFactory.CreateDbContext();
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                schedule.StartTime = start;
                schedule.EndTime = end;
                await _context.SaveChangesAsync();
            }
            return schedule;
        }

        public async Task DeleteScheduleAsync(Guid id)
        {
            using var _context = _contextFactory.CreateDbContext();
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
        }
    }
}