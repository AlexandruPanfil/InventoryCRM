using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryCRM.Dtos.Customers
{
    public record CustomerResponse(
        Guid Id,
        string IDNO,
        string IBAN,
        uint CodTVA,
        string Name,
        string Address,
        string PhoneNumber,
        string Email,
        string Description);

    public record CustomerCreateRequest
    {
        [Required]
        [RegularExpression(@"^[0-9]{13}$", ErrorMessage = "IDNO must be exactly 13 digits.")]
        public string IDNO { get; init; } = string.Empty;

        [Required]
        [RegularExpression(@"^[A-Z0-9]{24}$", ErrorMessage = "IBAN must be exactly 24 alphanumeric characters.")]
        public string IBAN { get; init; } = string.Empty;

        [Required]
        [Range(0, 9999999)]
        public uint CodTVA { get; init; }

        [Required]
        public string Name { get; init; } = string.Empty;

        public string Address { get; init; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; init; } = string.Empty;

        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;
    }

    public record CustomerUpdateRequest
    {
        [Required]
        [RegularExpression(@"^[0-9]{13}$", ErrorMessage = "IDNO must be exactly 13 digits.")]
        public string IDNO { get; init; } = string.Empty;

        [Required]
        [RegularExpression(@"^[A-Z0-9]{24}$", ErrorMessage = "IBAN must be exactly 24 alphanumeric characters.")]
        public string IBAN { get; init; } = string.Empty;

        [Required]
        [Range(0, 9999999)]
        public uint CodTVA { get; init; }

        [Required]
        public string Name { get; init; } = string.Empty;

        public string Address { get; init; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; init; } = string.Empty;

        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;
    }
}
