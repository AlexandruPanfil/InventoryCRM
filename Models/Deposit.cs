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
        public Guid? UserId { get; set; }
        public User? User { get; set; }

    }
}
