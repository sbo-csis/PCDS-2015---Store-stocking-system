using System.Collections.Generic;
using StoreStockingSystem.Models;

namespace PCDSWebsite.Models
{
    public class StoreViewModel
    {
        public StoreStockingSystem.Models.Store Store { get; set; }
        public List<StoreStockingSystem.Models.Stock> Stocks { get; set; }
        public PerformanceDetail Performance { get; set; }
    }
}