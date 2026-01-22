namespace InventoryCRM.Models
{
    public class Unit
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }

        // Foreign key to Deposit
        public Guid DepositId { get; set; }
        public Deposit Deposit { get; set; } = null!;
    }
}
