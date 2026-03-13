using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryCRM.Dtos.Deposits
{
    public record DepositResponse(Guid Id, string Name, bool IsMain, IReadOnlyList<DepositUnitResponse> Units);

    public record DepositUnitResponse(Guid Id, string Name, int Quantity, Guid DepositId);

    public record DepositCreateRequest
    {
        [Required]
        public string Name { get; init; } = string.Empty;
    }

    public record DepositUpdateRequest
    {
        [Required]
        public string Name { get; init; } = string.Empty;
    }

    public record DepositTransferRequest
    {
        [Required]
        public Guid SourceDepositId { get; init; }

        [Required]
        public Guid DestinationDepositId { get; init; }

        [Required]
        public List<TransferUnitRequest> Units { get; init; } = new();
    }

    public record TransferUnitRequest
    {
        public Guid Id { get; init; }

        [Required]
        public string Name { get; init; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; init; }
    }
}
