using StoreStockingSystem.Models;

namespace PCDSWebsite.Models
{
    public class RefillEntry
    {
        public Product Product { get; set; }
        public DisplayType DisplayType { get; set; }
        public int RefillCount { get; set; }
    }
}