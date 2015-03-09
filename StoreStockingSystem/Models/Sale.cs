using System;
using System.Data.Entity;

namespace StoreStockingSystem.Models
{
    public class Sale : DbContext
    {
        public virtual Store Store { get; set; }
        public Product Product { get; set; }
        public int SalesPrice { get; set; } // The price that a single product was sold for.
        public DisplayType DisplayType { get; set; }
        public DateTime SalesDate { get; set; }
    }
}