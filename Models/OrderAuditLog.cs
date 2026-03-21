namespace InventoryCRM.Models
{
    public class OrderAuditLog
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        // Nullable — system actions (e.g. automated transitions) have no user
        public string? UserId { get; set; }
        public string? UserName { get; set; }

        // e.g. "StatusChanged", "WorkerAssigned", "Created", "DescriptionUpdated"
        public string Action { get; set; } = string.Empty;

        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}