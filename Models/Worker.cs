namespace InventoryCRM.Models
{
    public class Worker
    {
        public Guid Id { get; set; }
        public string Workername { get; set; } = string.Empty;
        public Deposit? Deposit { get; set; }
    }
}
