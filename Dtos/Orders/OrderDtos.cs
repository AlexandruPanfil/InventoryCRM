using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryCRM.Dtos.Orders
{
    public record OrderResponse(
        Guid Id,
        uint OrderNumber,
        string Identifier,
        string Description,
        string Status,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        Guid CustomerId,
        string CustomerName,
        Guid? WorkerId,
        string? WorkerName,
        IReadOnlyList<UnitAssignmentSummary> UnitAssignments);

    public record OrderCreateRequest
    {
        [Required]
        public Guid CustomerId { get; init; }

        [Required]
        public string Description { get; init; } = string.Empty;

        public Guid? WorkerId { get; init; }

        public string? Status { get; init; }
    }

    public record OrderUpdateRequest
    {
        public string? Description { get; init; }

        public string? Status { get; init; }

        public Guid? WorkerId { get; init; }
    }

    public record UnitAssignmentSummary(Guid Id, string Name, int Quantity, string Status);
}
