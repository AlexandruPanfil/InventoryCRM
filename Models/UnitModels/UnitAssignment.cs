namespace InventoryCRM.Models.UnitModels
{
    public class UnitAssignment
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsExpanded { get; set; } = false;
        public string Status { get; set; } = UnitStatus.Reserved;

        // Foreign key to Customer
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public void SetStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status cannot be empty.", nameof(status));

            if (!UnitStatus.All.Contains(status))
                throw new ArgumentOutOfRangeException(nameof(status), $"Unknown status: {status}");

            Status = status;
        }
    }


}
