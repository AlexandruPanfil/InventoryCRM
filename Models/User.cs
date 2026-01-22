namespace InventoryCRM.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public Deposit? Deposit { get; set; }
    }
}
