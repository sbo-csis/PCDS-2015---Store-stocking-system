using System.Collections.Generic;

namespace StoreStockingSystem.Models
{
    public class StockRefill
    {
        public List<Stock> Stocks { get; set; }
        public string RefillResponseible { get; set; }
    }
}