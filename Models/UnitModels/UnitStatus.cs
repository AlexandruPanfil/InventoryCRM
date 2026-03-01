namespace InventoryCRM.Models.UnitModels
{
    public class UnitStatus
    {
        public const string Reserved = "Reserved";
        public const string Installed = "Installed";

        public static readonly IReadOnlyList<string> All = new[] { Reserved, Installed };
    }
}
