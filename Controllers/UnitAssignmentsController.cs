using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryCRM.Dtos.Units;
using InventoryCRM.Models.UnitModels;
using InventoryCRM.Services.UnitServices;
using Microsoft.AspNetCore.Mvc;

namespace InventoryCRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitAssignmentsController : ControllerBase
    {
        private readonly UnitAssignmentService _unitAssignmentService;

        public UnitAssignmentsController(UnitAssignmentService unitAssignmentService)
        {
            _unitAssignmentService = unitAssignmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitAssignmentResponse>>> GetUnitAssignmentsAsync(
            [FromQuery] Guid? customerId,
            [FromQuery] string? unitName)
        {
            List<UnitAssignment> units;
            if (customerId.HasValue)
            {
                units = await _unitAssignmentService.GetUnitsReservedByCustomerAsync(customerId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(unitName))
            {
                units = await _unitAssignmentService.FindUnitsReservedAsync(unitName.Trim());
            }
            else
            {
                units = await _unitAssignmentService.GetAllUnitsReservedAsync();
            }

            return Ok(units.Select(MapUnitAssignment));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UnitAssignmentResponse>> GetUnitAssignmentAsync(Guid id)
        {
            var unit = await _unitAssignmentService.GetUnitReservedAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            return Ok(MapUnitAssignment(unit));
        }

        [HttpPost]
        public async Task<ActionResult<UnitAssignmentResponse>> CreateUnitAssignmentAsync(UnitAssignmentCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var unit = await _unitAssignmentService.CreateUnitReservedAsync(
                request.Name.Trim(),
                request.Quantity,
                request.CustomerId);

            return CreatedAtAction("GetUnitAssignment", new { id = unit.Id }, MapUnitAssignment(unit));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UnitAssignmentResponse>> UpdateUnitAssignmentAsync(Guid id, UnitAssignmentUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var trimmedName = request.Name.Trim();
                var unit = await _unitAssignmentService.UpdateUnitReservedAsync(
                    id,
                    trimmedName,
                    request.Quantity,
                    request.Status);

                if (unit == null)
                {
                    return NotFound();
                }

                return Ok(MapUnitAssignment(unit));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUnitAssignmentAsync(Guid id)
        {
            var existing = await _unitAssignmentService.GetUnitReservedAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await _unitAssignmentService.DeleteUnitReservedAsync(id);
            return NoContent();
        }

        private static UnitAssignmentResponse MapUnitAssignment(UnitAssignment unit) => new(
            unit.Id,
            unit.Name,
            unit.Quantity,
            unit.Status,
            unit.CustomerId,
            unit.Customer?.Name ?? string.Empty);
    }
}
