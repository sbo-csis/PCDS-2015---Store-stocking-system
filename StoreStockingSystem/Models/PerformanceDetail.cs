using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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