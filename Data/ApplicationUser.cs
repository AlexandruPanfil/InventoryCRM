using InventoryCRM.Models;
using Microsoft.AspNetCore.Identity;

namespace InventoryCRM.Data
{
    public class ApplicationUser : IdentityUser
    {
        public Guid? WorkerId { get; set; }
        public Worker? Worker { get; set; }
    }
}
