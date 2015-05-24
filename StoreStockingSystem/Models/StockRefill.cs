using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreStockingSystem.Models
{
    public class StockRefill
    {
        public List<Stock> Stocks { get; set; }
        public string RefillResponseible { get; set; }
    }
}