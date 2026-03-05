using InventoryCRM.Models.UnitModels;

namespace InventoryCRM.Models
{
    public class Deposit
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsExpanded { get; set; } = false;
        public bool IsMain => Id == Guid.Parse("00000000-0000-0000-0000-000000000001");


        // Many to One to Units
        public ICollection<Unit>? Unit { get; set; }

        // Foreign key to User
        public Guid? WorkerId { get; set; }
        public Worker? Worker { get; set; }

    }
}
