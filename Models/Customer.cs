using System.ComponentModel.DataAnnotations;

namespace InventoryCRM.Models
{
    public class Customer
    {
        public Guid Id { get; set; }

        [RegularExpression(@"^[0-9]{13}$", ErrorMessage = "The value must be exactly 13 digits.")]
        public string IDNO { get; set; } = string.Empty;

        [RegularExpression(@"^[A-Z0-9]{24}$", ErrorMessage = "The value must be exactly 24 digits.")]
        public string IBAN { get; set; } = string.Empty;

        [RegularExpression(@"^[0-9]{7}$", ErrorMessage = "The value must be exactly 7 digits.")]
        public uint CodTVA { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Many to One to Orders
        public ICollection<Order>? Orders { get; set; }
    }
}
