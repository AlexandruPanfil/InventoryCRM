using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using InventoryCRM.Dtos.Deposits;
using InventoryCRM.Models;
using InventoryCRM.Models.UnitModels;
using InventoryCRM.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryCRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepositsController : ControllerBase
    {
        private readonly DepositService _depositService;

        public DepositsController(DepositService depositService)
        {
            _depositService = depositService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepositResponse>>> GetDepositsAsync([FromQuery] string? name)
        {
            var deposits = string.IsNullOrWhiteSpace(name)
                ? await _depositService.GetAllDepositsAsync()
                : await _depositService.FindDepositsAsync(name.Trim());

            return Ok(deposits.Select(MapDeposit));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DepositResponse>> GetDepositAsync(Guid id)
        {
            var deposits = await _depositService.GetOneDepositsAsync(id);
            var deposit = deposits.FirstOrDefault();
            if (deposit == null)
            {
                return NotFound();
            }

            return Ok(MapDeposit(deposit));
        }

        [HttpPost]
        public async Task<ActionResult<DepositResponse>> CreateDepositAsync(DepositCreateRequest request)
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

            var deposit = await _depositService.CreateDepositsAsync(trimmedName);
            return CreatedAtAction(nameof(GetDepositAsync), new { id = deposit.Id }, MapDeposit(deposit));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<DepositResponse>> UpdateDepositAsync(Guid id, DepositUpdateRequest request)
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

            var updated = await _depositService.UpdateDepositsAsync(id, trimmedName);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(MapDeposit(updated));
        }

        [HttpPut("transfer")]
        public async Task<ActionResult<DepositResponse>> TransferUnitsAsync(DepositTransferRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (request.SourceDepositId == request.DestinationDepositId)
            {
                ModelState.AddModelError(nameof(request.DestinationDepositId), "Source and destination deposits must differ.");
                return ValidationProblem(ModelState);
            }

            if (request.Units == null || request.Units.Count == 0)
            {
                ModelState.AddModelError(nameof(request.Units), "At least one unit must be included in the transfer.");
                return ValidationProblem(ModelState);
            }

            var unitsToTransfer = new List<Unit>();
            foreach (var transferUnit in request.Units)
            {
                if (string.IsNullOrWhiteSpace(transferUnit.Name))
                {
                    ModelState.AddModelError(nameof(transferUnit.Name), "Each unit must have a name.");
                    return ValidationProblem(ModelState);
                }

                if (transferUnit.Quantity <= 0)
                {
                    ModelState.AddModelError(nameof(transferUnit.Quantity), "Quantity must be positive.");
                    return ValidationProblem(ModelState);
                }

                unitsToTransfer.Add(new Unit
                {
                    Id = transferUnit.Id,
                    Name = transferUnit.Name.Trim(),
                    Quantity = transferUnit.Quantity,
                    DepositId = request.DestinationDepositId
                });
            }

            try
            {
                var destination = await _depositService.UpdateDepositsUnitsAsync(
                    request.SourceDepositId,
                    request.DestinationDepositId,
                    unitsToTransfer);

                return Ok(MapDeposit(destination));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteDepositAsync(Guid id)
        {
            var existing = await _depositService.GetDepositsAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await _depositService.DeleteDepositsAsync(id);
            return NoContent();
        }

        private static DepositResponse MapDeposit(Deposit deposit)
        {
            var units = deposit.Unit?
                .Select(u => new DepositUnitResponse(u.Id, u.Name, u.Quantity, u.DepositId))
                .ToList() ?? new List<DepositUnitResponse>();

            return new DepositResponse(deposit.Id, deposit.Name, deposit.IsMain, units);
        }
    }
}
