using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryCRM.Dtos.Units
{
    public record UnitAssignmentResponse(
        Guid Id,
        string Name,
        int Quantity,
        string Status,
        Guid CustomerId,
        string CustomerName);

    public record UnitAssignmentCreateRequest
    {
        [Required]
        public string Name { get; init; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; init; }

        [Required]
        public Guid CustomerId { get; init; }
    }

    public record UnitAssignmentUpdateRequest
    {
        [Required]
        public string Name { get; init; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; init; }

        public string? Status { get; init; }
    }
}
