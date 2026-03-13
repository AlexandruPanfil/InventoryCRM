using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using InventoryCRM.Dtos.Units;
using InventoryCRM.Models;
using InventoryCRM.Models.UnitModels;
using InventoryCRM.Services.UnitServices;
using Microsoft.AspNetCore.Mvc;

namespace InventoryCRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly UnitService _unitService;

        public UnitsController(UnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitResponse>>> GetUnitsAsync([FromQuery] string? name)
        {
            var units = string.IsNullOrWhiteSpace(name)
                ? await _unitService.GetAllUnitsAsync()
                : await _unitService.FindUnitsAsync(name.Trim());

            return Ok(units.Select(MapUnit));
        }

        [HttpGet("summary")]
        public async Task<ActionResult<IEnumerable<UnitSummary>>> GetUnitSummariesAsync()
        {
            var units = await _unitService.GetAllUnitsNameAndQuantityAsync();
            return Ok(units.Select(u => new UnitSummary(u.Name, u.Quantity)));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UnitResponse>> GetUnitAsync(Guid id)
        {
            var unit = await _unitService.GetUnitsAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            return Ok(MapUnit(unit));
        }

        [HttpGet("deposits")]
        public async Task<ActionResult<IEnumerable<UnitDepositResponse>>> GetDepositsByUnitNameAsync([FromQuery][Required] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(nameof(name), "Name is required.");
                return ValidationProblem(ModelState);
            }

            var deposits = await _unitService.GetDepositsByUnitNameAsync(name.Trim());
            return Ok(deposits.Select(d => new UnitDepositResponse(d.Id, d.Name, d.WorkerId)));
        }

        [HttpPost]
        public async Task<ActionResult<UnitResponse>> CreateUnitAsync(UnitCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var trimmedName = request.Name.Trim();
            if (trimmedName.Length == 0)
            {
                ModelState.AddModelError(nameof(request.Name), "Name cannot be empty or whitespace.");
                return ValidationProblem(ModelState);
            }

            var unit = await _unitService.CreateUnitsAsync(trimmedName, request.Quantity, request.DepositId);
            return CreatedAtAction(nameof(GetUnitAsync), new { id = unit.Id }, MapUnit(unit));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UnitResponse>> UpdateUnitAsync(Guid id, UnitUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var trimmedName = request.Name.Trim();
            if (trimmedName.Length == 0)
            {
                ModelState.AddModelError(nameof(request.Name), "Name cannot be empty or whitespace.");
                return ValidationProblem(ModelState);
            }

            var updated = await _unitService.UpdateUnitAsync(id, trimmedName, request.Quantity);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(MapUnit(updated));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUnitAsync(Guid id)
        {
            var existing = await _unitService.GetUnitsAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await _unitService.DeleteUnitAsync(id);
            return NoContent();
        }

        private static UnitResponse MapUnit(Unit unit)
        {
            return new UnitResponse(
                unit.Id,
                unit.Name,
                unit.Quantity,
                unit.DepositId,
                unit.Deposit?.Name ?? string.Empty);
        }
    }
}
