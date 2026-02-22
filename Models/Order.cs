using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryCRM.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Use OrderStatus constants and default to New
        public string Status { get; set; } = OrderStatus.New;

        // Many to One to Units
        public ICollection<Unit>? Unit { get; set; } = new List<Unit>();

        // Foreign key to Customers
        public Guid CustomersId { get; set; }
        public Customer Customers { get; set; } = null!;

        // Foreign key to User
        public Guid? UserId { get; set; }
        public User? User { get; set; }

        // Optional helper to set/validate status
        public void SetStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status cannot be empty.", nameof(status));

            if (!OrderStatus.All.Contains(status))
                throw new ArgumentOutOfRangeException(nameof(status), $"Unknown status: {status}");

            Status = status;
        }
    }
}
