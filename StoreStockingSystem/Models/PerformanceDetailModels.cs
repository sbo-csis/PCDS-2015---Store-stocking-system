using System.Collections.Generic;

namespace StoreStockingSystem.Models
{
    public class PerformanceDetail
    {
        public List<double> actualSales { get; set; }
        public List<double> predictedSales { get; set; }
        public List<Stock> stocks { get; set; }
    }
}