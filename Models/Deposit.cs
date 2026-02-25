using InventoryCRM.Models.UnitModels;

namespace InventoryCRM.Models
{
    public class Deposit
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsExpanded { get; set; } = false;


        // Many to One to Units
        public ICollection<Unit>? Unit { get; set; }

        // Foreign key to User
        public Guid? WorkerId { get; set; }
        public Worker? Worker { get; set; }

    }
}
