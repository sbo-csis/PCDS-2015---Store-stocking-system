using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreStockingSystem.Models
{
    public class ProductStock
    {
        public Stock Stock { get; set; }
        public Product Product { get; set; }
        public int Amount { get; set; }
    }
}