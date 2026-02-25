namespace InventoryCRM.Models.UnitModels
{
    public class UnitReserved
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsExpanded { get; set; } = false;

        // Foreign key to Customer
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
    }
}
