using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;

namespace InventoryManagement.Application.Contract.Inventory
{
    public class CreateInventory
    {
        public long ProductId { get; set; }
        public double UnitPrice { get; set; }
    }
}
