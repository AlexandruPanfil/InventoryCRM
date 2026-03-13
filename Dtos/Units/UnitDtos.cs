using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryCRM.Dtos.Units
{
    public record UnitResponse(Guid Id, string Name, int Quantity, Guid DepositId, string DepositName);

    public record UnitCreateRequest
    {
        [Required]
        public string Name { get; init; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; init; }

        [Required]
        public Guid DepositId { get; init; }
    }

    public record UnitUpdateRequest
    {
        [Required]
        public string Name { get; init; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; init; }
    }

    public record UnitSummary(string Name, int Quantity);

    public record UnitDepositResponse(Guid Id, string Name, Guid? WorkerId);
}
