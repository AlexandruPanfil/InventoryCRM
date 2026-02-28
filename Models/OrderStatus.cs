namespace InventoryCRM.Models
{
    public static class OrderStatus
    {
        public const string New = "New";
        //public const string Planned = "Planned";
        public const string InProcess = "InProcess";
        public const string Finished = "Finished";

        public static readonly IReadOnlyList<string> All = new[] { New, InProcess, Finished };
    }
}