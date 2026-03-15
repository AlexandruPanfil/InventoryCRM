using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InventoryCRM.Services;
using InventoryCRM.Models;
using InventoryCRM.Dtos.Schedules;

namespace InventoryCRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;

        public ScheduleController(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Schedule>>> GetAll()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            return Ok(schedules);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Schedule>> Get(Guid id)
        {
            var schedule = await _scheduleService.GetScheduleAsync(id);
            if (schedule == null) return NotFound();
            return Ok(schedule);
        }

        [HttpGet("range")]
        public async Task<ActionResult<List<Schedule>>> GetRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var schedules = await _scheduleService.GetSchedulesInRangeAsync(start, end);
            return Ok(schedules);
        }

        public class ScheduleDto
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult<Schedule>> Create([FromBody] ScheduleDto dto)
        {
            try
            {
                var created = await _scheduleService.CreateScheduleAsync(dto.StartTime, dto.EndTime);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Schedule>> Update(Guid id, [FromBody] ScheduleDto dto)
        {
            try
            {
                var updated = await _scheduleService.UpdateScheduleAsync(id, dto.StartTime, dto.EndTime);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _scheduleService.DeleteScheduleAsync(id);
            return NoContent();
        }
    }
}