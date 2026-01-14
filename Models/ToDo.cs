namespace InventoryCRM.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string? Todo { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime Date { get; set; }
    }
}
